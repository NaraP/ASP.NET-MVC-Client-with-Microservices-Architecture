namespace ECommerce.MVC.Models
{
    public class PaymentApiRequest
    {
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolder { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "Credit Card";
    }
}
