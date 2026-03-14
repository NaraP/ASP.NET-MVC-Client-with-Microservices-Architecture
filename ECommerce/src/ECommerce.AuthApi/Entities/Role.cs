using System.ComponentModel.DataAnnotations;

namespace ECommerce.AuthApi.Entities;

/// <summary>Defines a named role (Customer, Admin, Manager, etc.)</summary>
public class Role
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

/// <summary>Join table — many users can have many roles.</summary>
public class UserRole
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string UserId { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string AssignedBy { get; set; } = "system";

    // Navigation
    public AppUser User { get; set; } = null!;
    public Role    Role { get; set; } = null!;
}
