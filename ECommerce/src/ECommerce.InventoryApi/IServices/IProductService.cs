using ECommerce.InventoryApi.Controllers;
using ECommerce.InventoryApi.Dtos;
using ECommerce.InventoryApi.Models;

namespace ECommerce.InventoryApi.IServices
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(string? category, string? search, int page, int pageSize);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<ProductDto> CreateAsync(UpsertProductRequest req);
        Task<ProductDto?> UpdateAsync(int id, UpsertProductRequest req);
        Task<bool> UpdateStockAsync(int id, UpdateStockRequest req);
        Task<bool> DeleteAsync(int id);
    }
}
