using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MUNEEMJI.Models;

namespace MUNEEMJI.Services
{
    public class RazorpayService : IRazorpayService
    {
        private readonly RazorpaySettings _settings;
        private readonly ILogger<RazorpayService> _logger;

        private static readonly Dictionary<string, Dictionary<string, int>> PlanPrices = new()
        {
            ["silver"] = new Dictionary<string, int>
            {
                ["1 Month"] = 499,
                ["6 Months"] = 2499,
                ["1 Year"] = 4399,
                ["3 Years"] = 11999
            },
            ["gold"] = new Dictionary<string, int>
            {
                ["1 Month"] = 599,
                ["6 Months"] = 2999,
                ["1 Year"] = 4799,
                ["3 Years"] = 13499
            }
        };

        public RazorpayService(IOptions<RazorpaySettings> settings, ILogger<RazorpayService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<RazorpayOrderResult> CreateOrderAsync(string planName, string duration, string platform)
        {
            try
            {
                var plan = planName.ToLower();
                if (!PlanPrices.ContainsKey(plan) || !PlanPrices[plan].ContainsKey(duration))
                {
                    return new RazorpayOrderResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid plan or duration selected."
                    };
                }

                if (string.IsNullOrEmpty(_settings.KeyId) || string.IsNullOrEmpty(_settings.KeySecret))
                {
                    _logger.LogError("Razorpay KeyId or KeySecret is not configured. KeyId: '{KeyId}', KeySecret length: {Len}",
                        _settings.KeyId ?? "NULL", _settings.KeySecret?.Length ?? 0);
                    return new RazorpayOrderResult
                    {
                        Success = false,
                        ErrorMessage = "Payment gateway is not configured. Please contact support."
                    };
                }

                int amountInRupees = PlanPrices[plan][duration];
                int amountInPaise = amountInRupees * 100;

                var orderPayload = new
                {
                    amount = amountInPaise,
                    currency = "INR",
                    receipt = $"rcpt_{plan}_{DateTime.UtcNow.Ticks}"
                };

                var jsonContent = JsonSerializer.Serialize(orderPayload);

                _logger.LogInformation("Creating Razorpay order - KeyId: {KeyId}, Secret length: {SecretLen}, Amount: {Amount}",
                    _settings.KeyId, _settings.KeySecret.Length, amountInPaise);

                // Simple HttpClient - no handler credentials, just explicit auth header
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Clear();

                // Build Basic auth: base64(keyId:keySecret)
                var credentials = $"{_settings.KeyId}:{_settings.KeySecret}";
                var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending to Razorpay. Auth header: Basic {Auth}", base64Credentials);

                var response = await client.PostAsync("https://api.razorpay.com/v1/orders", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Razorpay response - Status: {Status}, Body: {Body}", response.StatusCode, responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    return new RazorpayOrderResult
                    {
                        Success = false,
                        ErrorMessage = $"Razorpay returned {response.StatusCode}: {responseBody}"
                    };
                }

                using var doc = JsonDocument.Parse(responseBody);
                var orderId = doc.RootElement.GetProperty("id").GetString();

                _logger.LogInformation("Razorpay order created: {OrderId}", orderId);

                return new RazorpayOrderResult
                {
                    Success = true,
                    OrderId = orderId,
                    AmountInPaise = amountInPaise,
                    Currency = "INR",
                    PlanName = planName,
                    Duration = duration
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Razorpay order");
                return new RazorpayOrderResult
                {
                    Success = false,
                    ErrorMessage = $"Exception: {ex.Message}"
                };
            }
        }

        public bool VerifyPaymentSignature(string orderId, string paymentId, string signature)
        {
            try
            {
                var payload = $"{orderId}|{paymentId}";
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.KeySecret));
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();
                return string.Equals(computedSignature, signature, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Razorpay signature");
                return false;
            }
        }
    }
}
