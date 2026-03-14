using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using System.Text.Json;

namespace ECommerce.MVC.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public InventoryService(HttpClient http) => _http = http;

        public async Task<List<ProductViewModel>> GetProductsAsync(string? category = null, string? search = null)
        {
            var url = "/api/Products?page=1&pageSize=100";

            if (!string.IsNullOrWhiteSpace(category)) url += $"&category={Uri.EscapeDataString(category)}";
            if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";

            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();

            // API now returns a PagedResult<ProductDto> wrapper
            var paged = JsonSerializer.Deserialize<PagedResponse<ProductViewModel>>(json, _json);
            return paged?.Items ?? new();
        }

        public async Task<ProductViewModel?> GetProductAsync(int id)
        {
            var response = await _http.GetAsync($"/api/products/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductViewModel>(json, _json);
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var response = await _http.GetAsync("/api/products/categories");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<string>>(json, _json) ?? new();
        }
    }
}
