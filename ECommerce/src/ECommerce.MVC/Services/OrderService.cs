using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using System.Text;
using System.Text.Json;

namespace ECommerce.MVC.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public OrderService(HttpClient http) => _http = http;

        public async Task<OrderApiResponse?> CreateOrderAsync(CreateOrderApiRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/orders", content);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderApiResponse>(json, _json);
        }
    }
}
