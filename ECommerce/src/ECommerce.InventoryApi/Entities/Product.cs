using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.InventoryApi.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Column(TypeName = "decimal(3,1)")]
    public decimal Rating { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
