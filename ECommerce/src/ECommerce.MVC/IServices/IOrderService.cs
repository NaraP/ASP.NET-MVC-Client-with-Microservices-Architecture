using ECommerce.MVC.Models;
using ECommerce.MVC.Services;

namespace ECommerce.MVC.IServices
{
    public interface IOrderService
    {
        Task<OrderApiResponse?> CreateOrderAsync(CreateOrderApiRequest request);
    }
}
