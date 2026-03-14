using ECommerce.MVC.Services;

namespace ECommerce.MVC.Models
{
    public class CreateOrderApiRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<OrderItemApi> Items { get; set; } = [];
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentReference { get; set; } = string.Empty;
    }
}
