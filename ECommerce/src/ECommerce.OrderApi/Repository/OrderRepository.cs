using ECommerce.OrderApi.Data;
using ECommerce.OrderApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderApi.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _orderDbContext;

        public OrderRepository(OrderDbContext orderDbContext) 
        {
            _orderDbContext = orderDbContext;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _orderDbContext.Orders.Add(order);
            await _orderDbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            return await _orderDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId)
        {
            return await _orderDbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(string orderId, string status)
        {
            var order = await _orderDbContext.Orders.FindAsync(orderId);

            if (order == null)
                return false;

            order.Status = status;
            await _orderDbContext.SaveChangesAsync();

            return true;
        }
    }
}