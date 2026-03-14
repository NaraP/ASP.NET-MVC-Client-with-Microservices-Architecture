using ECommerce.InventoryApi.IServices;
using ECommerce.InventoryApi.Repository;
using ECommerce.InventoryApi.Services;

namespace ECommerce.InventoryApi
{
    public static class RegisterService
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Repository Layer
            services.AddScoped<IProductRepository, ProductRepository>();
            // Service Layer
            services.AddScoped<IProductService, ProductService>();
        }
    }
}
