using ECommerce.InventoryApi.Data;
using ECommerce.InventoryApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.InventoryApi.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly InventoryDbContext _db;

        public ProductRepository(InventoryDbContext db)
        {
            _db = db;
        }

        public async Task<(IEnumerable<Product> Items, int Total)> GetAllAsync(string? category, string? search, int page, int pageSize)
        {
            var query = _db.Products.Where(p => p.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category == category);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _db.Products
                .Where(p => p.IsActive)
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateStockAsync(int id, int quantity)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null || product.Stock < quantity)
                return false;

            product.Stock -= quantity;
            product.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
