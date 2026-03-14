namespace ECommerce.MVC.Models
{
    public class PaymentApiResponse
    {
        public string PaymentReference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? CardLastFour { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
