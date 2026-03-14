using ECommerce.PaymentApi.IServices;
using ECommerce.PaymentApi.Repository;
using ECommerce.PaymentApi.Services;

namespace ECommerce.PaymentApi
{
    public static class RegisterService
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Repository Layer
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            // Service Layer
            services.AddScoped<IPaymentService, PaymentService>();
        }
    }
}
