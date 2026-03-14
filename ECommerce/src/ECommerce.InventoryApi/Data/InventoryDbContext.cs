using ECommerce.InventoryApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.InventoryApi.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Category);
            entity.HasIndex(p => p.Name);

            // Seed 12 dummy products
            entity.HasData(
                new Product { Id = 1,  Name = "MacBook Pro 14\"",       Category = "Laptops",     Price = 1999.99m, Stock = 15, Rating = 4.9m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "Apple M3 Pro chip, 18GB RAM, 512GB SSD",                    ImageUrl = "https://placehold.co/300x200?text=MacBook+Pro" },
                new Product { Id = 2,  Name = "Dell XPS 15",            Category = "Laptops",     Price = 1499.99m, Stock = 20, Rating = 4.7m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "Intel Core i7, 16GB RAM, 512GB NVMe SSD",                  ImageUrl = "https://placehold.co/300x200?text=Dell+XPS+15" },
                new Product { Id = 3,  Name = "iPhone 16 Pro",          Category = "Phones",      Price = 1199.99m, Stock = 50, Rating = 4.8m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "A18 Pro chip, 256GB, Titanium finish",                     ImageUrl = "https://placehold.co/300x200?text=iPhone+16+Pro" },
                new Product { Id = 4,  Name = "Samsung Galaxy S25",     Category = "Phones",      Price = 999.99m,  Stock = 45, Rating = 4.6m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "Snapdragon 8 Elite, 12GB RAM, 256GB",                      ImageUrl = "https://placehold.co/300x200?text=Samsung+S25" },
                new Product { Id = 5,  Name = "Sony WH-1000XM5",        Category = "Audio",       Price = 349.99m,  Stock = 30, Rating = 4.8m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "Industry-leading noise cancellation, 30hr battery",       ImageUrl = "https://placehold.co/300x200?text=Sony+WH1000XM5" },
                new Product { Id = 6,  Name = "Apple Watch Ultra 2",    Category = "Wearables",   Price = 799.99m,  Stock = 25, Rating = 4.7m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "49mm Titanium case, GPS + Cellular",                      ImageUrl = "https://placehold.co/300x200?text=Apple+Watch+Ultra" },
                new Product { Id = 7,  Name = "iPad Pro 12.9\"",        Category = "Tablets",     Price = 1099.99m, Stock = 18, Rating = 4.9m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "M4 chip, Liquid Retina XDR display, 256GB",               ImageUrl = "https://placehold.co/300x200?text=iPad+Pro" },
                new Product { Id = 8,  Name = "LG 27\" 4K Monitor",     Category = "Monitors",    Price = 599.99m,  Stock = 12, Rating = 4.6m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "27\" UHD IPS, 144Hz, HDR600",                            ImageUrl = "https://placehold.co/300x200?text=LG+4K+Monitor" },
                new Product { Id = 9,  Name = "Logitech MX Keys",       Category = "Accessories", Price = 109.99m,  Stock = 60, Rating = 4.5m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "Advanced wireless keyboard, multi-device",               ImageUrl = "https://placehold.co/300x200?text=MX+Keys" },
                new Product { Id = 10, Name = "Razer DeathAdder V3",    Category = "Accessories", Price = 79.99m,   Stock = 75, Rating = 4.7m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "Ergonomic gaming mouse, 30K DPI sensor",                 ImageUrl = "https://placehold.co/300x200?text=Razer+Mouse" },
                new Product { Id = 11, Name = "Nintendo Switch OLED",   Category = "Gaming",      Price = 349.99m,  Stock = 35, Rating = 4.8m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "7\" OLED screen, 64GB storage, dock included",            ImageUrl = "https://placehold.co/300x200?text=Nintendo+Switch" },
                new Product { Id = 12, Name = "GoPro Hero 13 Black",    Category = "Cameras",     Price = 399.99m,  Stock = 22, Rating = 4.6m, IsActive = true, CreatedAt = new DateTime(2025, 1, 1), Description = "5.3K video, HyperSmooth 7.0, waterproof",               ImageUrl = "https://placehold.co/300x200?text=GoPro+Hero13" }
            );
        });
    }
}
