namespace ECommerce.MVC.Models
{
    public class OrderApiResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public List<OrderItemApi> Items { get; set; } = new();
    }
}
