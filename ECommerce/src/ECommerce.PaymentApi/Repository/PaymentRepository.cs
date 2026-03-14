using ECommerce.PaymentApi.Data;
using ECommerce.PaymentApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.PaymentApi.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _db;

        public PaymentRepository(PaymentDbContext db)
        {
            _db = db;
        }

        public async Task AddPaymentAsync(PaymentRecord payment)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
        }

        public async Task<PaymentRecord?> GetPaymentAsync(string paymentReference)
        {
            return await _db.Payments.FindAsync(paymentReference);
        }

        public async Task<IEnumerable<PaymentRecord>> GetPaymentsByUserAsync(string userId)
        {
            return await _db.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.ProcessedAt)
                .ToListAsync();
        }

        public async Task UpdatePaymentAsync(PaymentRecord payment)
        {
            _db.Payments.Update(payment);
            await _db.SaveChangesAsync();
        }
    }
}
