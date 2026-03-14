using ECommerce.MVC.Models;
using ECommerce.MVC.Services;

namespace ECommerce.MVC.IServices
{
    public interface IPaymentService
    {
        Task<PaymentApiResponse?> ProcessPaymentAsync(PaymentApiRequest request);
    }
}
