using ECommerce.OrderApi.Controllers;

namespace ECommerce.OrderApi.Models
{
    public class OrderResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentReference { get; set; } = string.Empty;
        public List<Dtos.OrderItemDto> Items { get; set; } = new();
    }
}
