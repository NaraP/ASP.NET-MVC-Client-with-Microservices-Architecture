using ECommerce.PaymentApi.Entities;
using ECommerce.PaymentApi.IServices;
using ECommerce.PaymentApi.Models;
using ECommerce.PaymentApi.Repository;

namespace ECommerce.PaymentApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;

        public PaymentService(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            await Task.Delay(800);

            var isSuccess = !request.CardNumber.Replace(" ", "").EndsWith("0000");

            var paymentRef = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
            var cardLastFour = request.CardNumber.Replace(" ", "")[^4..];

            var record = new PaymentRecord
            {
                PaymentReference = paymentRef,
                UserId = request.UserId,
                Amount = request.Amount,
                Currency = request.Currency ?? "USD",
                CardLastFour = cardLastFour,
                Status = isSuccess ? "Success" : "Failed",
                FailureReason = isSuccess ? null : "Card declined. Please check your card details.",
                ProcessedAt = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod ?? "Credit Card"
            };

            await _repository.AddPaymentAsync(record);

            return new PaymentResponse
            {
                PaymentReference = paymentRef,
                Status = record.Status,
                Message = isSuccess ? "Payment processed successfully." : record.FailureReason!,
                Amount = request.Amount,
                Currency = record.Currency,
                CardLastFour = cardLastFour,
                ProcessedAt = record.ProcessedAt
            };
        }

        public async Task<PaymentRecord?> GetPaymentAsync(string paymentReference)
        {
            return await _repository.GetPaymentAsync(paymentReference);
        }

        public async Task<IEnumerable<PaymentRecord>> GetPaymentsByUserAsync(string userId)
        {
            return await _repository.GetPaymentsByUserAsync(userId);
        }

        public async Task<bool> LinkOrderAsync(string paymentReference, string orderId)
        {
            var payment = await _repository.GetPaymentAsync(paymentReference);

            if (payment == null)
                return false;

            payment.OrderId = orderId;

            await _repository.UpdatePaymentAsync(payment);

            return true;
        }
    }
}
