using System.ComponentModel.DataAnnotations;

namespace ECommerce.AuthApi.Entities;

/// <summary>Persisted refresh token — allows getting new access tokens without re-login.</summary>
public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Token { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string UserId { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? ReplacedByToken { get; set; }

    // Navigation
    public AppUser User { get; set; } = null!;
}
