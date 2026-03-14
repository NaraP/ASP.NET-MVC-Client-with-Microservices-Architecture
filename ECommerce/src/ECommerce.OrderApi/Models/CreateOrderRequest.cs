using ECommerce.OrderApi.Dtos;

namespace ECommerce.OrderApi.Models
{
    public class CreateOrderRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentReference { get; set; } = string.Empty;
    }
}
