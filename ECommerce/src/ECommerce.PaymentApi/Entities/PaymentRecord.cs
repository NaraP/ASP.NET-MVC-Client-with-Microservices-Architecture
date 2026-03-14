using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.PaymentApi.Entities;

public class PaymentRecord
{
    [Key, MaxLength(100)]
    public string PaymentReference { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string UserId { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    [MaxLength(4)]
    public string CardLastFour { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Status { get; set; } = string.Empty;          // Success | Failed

    [MaxLength(500)]
    public string? FailureReason { get; set; }

    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "Credit Card";

    [MaxLength(50)]
    public string? OrderId { get; set; }   // linked after order is created
}
