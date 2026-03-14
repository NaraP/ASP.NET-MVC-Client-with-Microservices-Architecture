using ECommerce.InventoryApi.Entities;

namespace ECommerce.InventoryApi.Repository
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> Items, int Total)> GetAllAsync(string? category, string? search, int page, int pageSize);
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> UpdateStockAsync(int id, int quantity);
        Task<bool> SoftDeleteAsync(int id);
    }
}
