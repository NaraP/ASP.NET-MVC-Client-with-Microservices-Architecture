using ECommerce.PaymentApi.Entities;
using ECommerce.PaymentApi.Models;

namespace ECommerce.PaymentApi.IServices
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
        Task<PaymentRecord?> GetPaymentAsync(string paymentReference);
        Task<IEnumerable<PaymentRecord>> GetPaymentsByUserAsync(string userId);
        Task<bool> LinkOrderAsync(string paymentReference, string orderId);
    }
}
