using MUNEEMJI.Models;

namespace MUNEEMJI.Services
{
    public interface IRazorpayService
    {
        Task<RazorpayOrderResult> CreateOrderAsync(string planName, string duration, string platform);
        bool VerifyPaymentSignature(string orderId, string paymentId, string signature);
    }

    public class RazorpayOrderResult
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public int AmountInPaise { get; set; }
        public string Currency { get; set; }
        public string PlanName { get; set; }
        public string Duration { get; set; }
        public string ErrorMessage { get; set; }
    }
}
