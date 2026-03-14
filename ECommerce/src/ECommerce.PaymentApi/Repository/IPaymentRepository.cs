using ECommerce.PaymentApi.Entities;

namespace ECommerce.PaymentApi.Repository
{
    public interface IPaymentRepository
    {
        Task AddPaymentAsync(PaymentRecord payment);
        Task<PaymentRecord?> GetPaymentAsync(string paymentReference);
        Task<IEnumerable<PaymentRecord>> GetPaymentsByUserAsync(string userId);
        Task UpdatePaymentAsync(PaymentRecord payment);
    }
}
