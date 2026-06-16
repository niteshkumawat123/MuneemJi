using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MUNEEMJI.Models;
using MUNEEMJI.Services;
using System.Net.Http.Headers;
using System.Text;

namespace MUNEEMJI.Controllers
{
    public class PlansAndPricingController : Controller
    {
        private readonly IRazorpayService _razorpayService;
        private readonly RazorpaySettings _razorpaySettings;

        public PlansAndPricingController(IRazorpayService razorpayService, IOptions<RazorpaySettings> razorpaySettings)
        {
            _razorpayService = razorpayService;
            _razorpaySettings = razorpaySettings.Value;
        }

        // Test endpoint - hit /Web/PlansAndPricing/TestRazorpay in browser to verify keys
        [HttpGet]
        public async Task<IActionResult> TestRazorpay()
        {
            try
            {
                var keyId = _razorpaySettings.KeyId;
                var keySecret = _razorpaySettings.KeySecret;

                var diagnostics = new StringBuilder();
                diagnostics.AppendLine($"KeyId from config: '{keyId}'");
                diagnostics.AppendLine($"KeyId length: {keyId?.Length ?? 0}");
                diagnostics.AppendLine($"KeySecret length: {keySecret?.Length ?? 0}");
                diagnostics.AppendLine($"KeySecret first 4 chars: '{keySecret?.Substring(0, Math.Min(4, keySecret?.Length ?? 0))}'");

                var credentials = $"{keyId}:{keySecret}";
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
                diagnostics.AppendLine($"Base64 auth: {base64}");

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);

                // Simple GET to fetch orders (requires valid auth)
                var response = await client.GetAsync("https://api.razorpay.com/v1/payments?count=1");
                var body = await response.Content.ReadAsStringAsync();

                diagnostics.AppendLine($"HTTP Status: {response.StatusCode}");
                diagnostics.AppendLine($"Response: {body}");

                return Content(diagnostics.ToString(), "text/plain");
            }
            catch (Exception ex)
            {
                return Content($"Exception: {ex.Message}\n{ex.StackTrace}", "text/plain");
            }
        }

        public IActionResult Index()
        {
            ViewBag.RazorpayKeyId = _razorpaySettings.KeyId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PlanName) || string.IsNullOrEmpty(request.Duration))
            {
                return Json(new { success = false, message = "Invalid request. Please select a plan and duration." });
            }

            var result = await _razorpayService.CreateOrderAsync(request.PlanName, request.Duration, request.Platform);

            if (!result.Success)
            {
                return Json(new { success = false, message = result.ErrorMessage });
            }

            return Json(new
            {
                success = true,
                orderId = result.OrderId,
                amount = result.AmountInPaise,
                currency = result.Currency,
                planName = result.PlanName,
                duration = result.Duration
            });
        }

        [HttpPost]
        public IActionResult VerifyPayment([FromBody] PaymentVerificationRequest request)
        {
            if (request == null ||
                string.IsNullOrEmpty(request.razorpay_order_id) ||
                string.IsNullOrEmpty(request.razorpay_payment_id) ||
                string.IsNullOrEmpty(request.razorpay_signature))
            {
                return Json(new { success = false, message = "Invalid payment data." });
            }

            var isValid = _razorpayService.VerifyPaymentSignature(
                request.razorpay_order_id,
                request.razorpay_payment_id,
                request.razorpay_signature);

            if (isValid)
            {
                return Json(new
                {
                    success = true,
                    message = "Payment verified successfully!",
                    paymentId = request.razorpay_payment_id,
                    orderId = request.razorpay_order_id
                });
            }

            return Json(new { success = false, message = "Payment verification failed. Please contact support." });
        }

        [HttpPost]
        public IActionResult UpgradeToPro()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Business()
        {
            return View();
        }

        public IActionResult Preferences()
        {
            return View();
        }

        public IActionResult Support()
        {
            return View();
        }
    }
}
