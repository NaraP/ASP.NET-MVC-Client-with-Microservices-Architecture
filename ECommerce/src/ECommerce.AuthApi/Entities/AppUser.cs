using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.AuthApi.Entities;

public class AppUser
{
    [Key, MaxLength(50)]
    public string Id { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool EmailVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation — one user has many role assignments
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
