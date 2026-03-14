using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.InventoryApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Rating", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Laptops", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Apple M3 Pro chip, 18GB RAM, 512GB SSD", "https://placehold.co/300x200?text=MacBook+Pro", true, "MacBook Pro 14\"", 1999.99m, 4.9m, 15, null },
                    { 2, "Laptops", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Intel Core i7, 16GB RAM, 512GB NVMe SSD", "https://placehold.co/300x200?text=Dell+XPS+15", true, "Dell XPS 15", 1499.99m, 4.7m, 20, null },
                    { 3, "Phones", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A18 Pro chip, 256GB, Titanium finish", "https://placehold.co/300x200?text=iPhone+16+Pro", true, "iPhone 16 Pro", 1199.99m, 4.8m, 50, null },
                    { 4, "Phones", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Snapdragon 8 Elite, 12GB RAM, 256GB", "https://placehold.co/300x200?text=Samsung+S25", true, "Samsung Galaxy S25", 999.99m, 4.6m, 45, null },
                    { 5, "Audio", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Industry-leading noise cancellation, 30hr battery", "https://placehold.co/300x200?text=Sony+WH1000XM5", true, "Sony WH-1000XM5", 349.99m, 4.8m, 30, null },
                    { 6, "Wearables", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "49mm Titanium case, GPS + Cellular", "https://placehold.co/300x200?text=Apple+Watch+Ultra", true, "Apple Watch Ultra 2", 799.99m, 4.7m, 25, null },
                    { 7, "Tablets", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "M4 chip, Liquid Retina XDR display, 256GB", "https://placehold.co/300x200?text=iPad+Pro", true, "iPad Pro 12.9\"", 1099.99m, 4.9m, 18, null },
                    { 8, "Monitors", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "27\" UHD IPS, 144Hz, HDR600", "https://placehold.co/300x200?text=LG+4K+Monitor", true, "LG 27\" 4K Monitor", 599.99m, 4.6m, 12, null },
                    { 9, "Accessories", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced wireless keyboard, multi-device", "https://placehold.co/300x200?text=MX+Keys", true, "Logitech MX Keys", 109.99m, 4.5m, 60, null },
                    { 10, "Accessories", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ergonomic gaming mouse, 30K DPI sensor", "https://placehold.co/300x200?text=Razer+Mouse", true, "Razer DeathAdder V3", 79.99m, 4.7m, 75, null },
                    { 11, "Gaming", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "7\" OLED screen, 64GB storage, dock included", "https://placehold.co/300x200?text=Nintendo+Switch", true, "Nintendo Switch OLED", 349.99m, 4.8m, 35, null },
                    { 12, "Cameras", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5.3K video, HyperSmooth 7.0, waterproof", "https://placehold.co/300x200?text=GoPro+Hero13", true, "GoPro Hero 13 Black", 399.99m, 4.6m, 22, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
