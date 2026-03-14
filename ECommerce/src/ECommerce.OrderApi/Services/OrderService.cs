using ECommerce.OrderApi.Dtos;
using ECommerce.OrderApi.Entities;
using ECommerce.OrderApi.IServices;
using ECommerce.OrderApi.Models;
using ECommerce.OrderApi.Repository;

namespace ECommerce.OrderApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            var orderId = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(10000, 99999)}";

            var order = new Order
            {
                OrderId = orderId,
                UserId = request.UserId,
                UserEmail = request.UserEmail,
                TotalAmount = request.TotalAmount,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow,
                ShippingAddress = request.ShippingAddress,
                PaymentReference = request.PaymentReference,
                EstimatedDelivery = DateTime.UtcNow.AddDays(Random.Shared.Next(3, 7)),
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

            var created = await _repository.CreateOrderAsync(order);

            return MapToResponse(created);
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(string orderId)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);

            return order == null ? null : MapToResponse(order);
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersByUserAsync(string userId)
        {
            var orders = await _repository.GetOrdersByUserAsync(userId);

            return orders.Select(MapToResponse);
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string status)
        {
            return await _repository.UpdateStatusAsync(orderId, status);
        }

        private static OrderResponse MapToResponse(Order o)
        {
            return new OrderResponse
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                UserEmail = o.UserEmail,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                EstimatedDelivery = o.EstimatedDelivery,
                ShippingAddress = o.ShippingAddress,
                PaymentReference = o.PaymentReference,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };
        }
    }
}
