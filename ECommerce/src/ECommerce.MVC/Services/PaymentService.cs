using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using System.Text;
using System.Text.Json;

namespace ECommerce.MVC.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public PaymentService(HttpClient http) => _http = http;

        public async Task<PaymentApiResponse?> ProcessPaymentAsync(PaymentApiRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/payments/process", content);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentApiResponse>(json, _json);
        }
    }
}
