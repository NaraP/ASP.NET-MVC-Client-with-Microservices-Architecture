using ECommerce.OrderApi.Entities;

namespace ECommerce.OrderApi.Repository
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId);
        Task<bool> UpdateStatusAsync(string orderId, string status);
    }
}
