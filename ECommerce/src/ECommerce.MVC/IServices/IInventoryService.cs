using ECommerce.MVC.Models;

namespace ECommerce.MVC.IServices
{
    public interface IInventoryService
    {
        Task<List<ProductViewModel>> GetProductsAsync(string? category = null, string? search = null);
        Task<ProductViewModel?> GetProductAsync(int id);
        Task<List<string>> GetCategoriesAsync();
    }
}
