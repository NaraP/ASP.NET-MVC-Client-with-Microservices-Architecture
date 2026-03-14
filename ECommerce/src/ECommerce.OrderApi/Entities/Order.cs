using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.OrderApi.Entities;

public class Order
{
    [Key, MaxLength(50)]
    public string OrderId { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string UserId { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string UserEmail { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required, MaxLength(50)]
    public string Status { get; set; } = "Confirmed";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime EstimatedDelivery { get; set; }

    [Required, MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    public string PaymentReference { get; set; } = string.Empty;

    // Navigation
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    // FK to Order
    [MaxLength(50)]
    public string OrderId { get; set; } = string.Empty;

    public int ProductId { get; set; }

    [Required, MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
}
