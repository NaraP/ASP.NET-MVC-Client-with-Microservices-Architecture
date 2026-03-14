using ECommerce.OrderApi.IServices;
using ECommerce.OrderApi.Repository;
using ECommerce.OrderApi.Services;

namespace ECommerce.OrderApi
{
    public static class RegisterService
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Repository Layer
            services.AddScoped<IOrderRepository, OrderRepository>();
            // Service Layer
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}
