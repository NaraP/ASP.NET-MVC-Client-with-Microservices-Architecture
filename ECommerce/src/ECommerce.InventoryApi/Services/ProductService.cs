using ECommerce.InventoryApi.Controllers;
using ECommerce.InventoryApi.Dtos;
using ECommerce.InventoryApi.Entities;
using ECommerce.InventoryApi.IServices;
using ECommerce.InventoryApi.Models;
using ECommerce.InventoryApi.Repository;

namespace ECommerce.InventoryApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(string? category, string? search, int page, int pageSize)
        {
            var (items, total) = await _repo.GetAllAsync(category, search, page, pageSize);

            var dtos = items.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                Stock = p.Stock,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Rating = Convert.ToDouble(p.Rating)
            });

            return new PagedResult<ProductDto>
            {
                Items = dtos.ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _repo.GetByIdAsync(id);

            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                Stock = p.Stock,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Rating = (double)p.Rating
            };
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _repo.GetCategoriesAsync();
        }

        public async Task<ProductDto> CreateAsync(UpsertProductRequest req)
        {
            var product = new Product
            {
                Name = req.Name,
                Category = req.Category,
                Price = req.Price,
                Stock = req.Stock,
                Description = req.Description,
                ImageUrl = req.ImageUrl,
                Rating = req.Rating,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.CreateAsync(product);

            return new ProductDto
            {
                Id = created.Id,
                Name = created.Name,
                Category = created.Category,
                Price = created.Price,
                Stock = created.Stock,
                Description = created.Description,
                ImageUrl = created.ImageUrl,
                Rating = (double)created.Rating
            };
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpsertProductRequest req)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null) return null;

            product.Name = req.Name;
            product.Category = req.Category;
            product.Price = req.Price;
            product.Stock = req.Stock;
            product.Description = req.Description;
            product.ImageUrl = req.ImageUrl;
            product.Rating = req.Rating;
            product.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(product);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Stock = product.Stock,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Rating = (double)product.Rating
            };
        }

        public async Task<bool> UpdateStockAsync(int id, UpdateStockRequest req)
        {
            return await _repo.UpdateStockAsync(id, req.Quantity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.SoftDeleteAsync(id);
        }
    }
}
