using ECommerce.OrderApi.Models;

namespace ECommerce.OrderApi.IServices
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderResponse?> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<OrderResponse>> GetOrdersByUserAsync(string userId);
        Task<bool> UpdateOrderStatusAsync(string orderId, string status);
    }
}
