using System.ComponentModel.DataAnnotations;

namespace ECommerce.AuthApi.Models;

// ── Auth ─────────────────────────────────────────────────────────────────────

public class RegisterRequest
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public string? RoleName { get; set; }

    [Required, MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must have uppercase, lowercase, and a digit.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must have uppercase, lowercase, and a digit.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

// ── Roles ────────────────────────────────────────────────────────────────────

public class AssignRoleRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string RoleName { get; set; } = string.Empty;
}

public class RemoveRoleRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string RoleName { get; set; } = string.Empty;
}

public class CreateRoleRequest
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
}

// ── Users (Admin) ─────────────────────────────────────────────────────────────

public class UpdateUserRequest
{
    [MaxLength(100)] public string? FirstName   { get; set; }
    [MaxLength(100)] public string? LastName    { get; set; }
    [MaxLength(20)]  public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
}

// ── Responses ────────────────────────────────────────────────────────────────

public class AuthResponse
{
    public string       AccessToken   { get; set; } = string.Empty;
    public string       RefreshToken  { get; set; } = string.Empty;
    public DateTime     ExpiresAt     { get; set; }
    public string       TokenType     { get; set; } = "Bearer";
    public UserDto      User          { get; set; } = new();
}

public class UserDto
{
    public string       Id          { get; set; } = string.Empty;
    public string       FullName    { get; set; } = string.Empty;
    public string       Email       { get; set; } = string.Empty;
    public string       PhoneNumber { get; set; } = string.Empty;
    public List<string> Roles       { get; set; } = new();
    public bool         IsActive    { get; set; }
    public DateTime     CreatedAt   { get; set; }
    public DateTime?    LastLoginAt { get; set; }
}

public class RoleDto
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool   IsActive    { get; set; }
    public int    UserCount   { get; set; }
}
