namespace ECommerce.MVC.Models
{
    // ═══════════════════════════════════════════════════════════════════════════════
    //  DTOs — mirror the shapes returned by ECommerce.AuthApi
    //  These live in the MVC layer so AuthApi has no project reference dependency.
    // ═══════════════════════════════════════════════════════════════════════════════

    /// <summary>Full response from POST /api/auth/login or /api/auth/register.</summary>
    public class AuthApiResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public AuthUserDto User { get; set; } = new();
    }

    /// <summary>User profile embedded inside every AuthApi response.</summary>
    public class AuthUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    /// <summary>Sent to POST /api/auth/login.</summary>
    public class AuthLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>Sent to POST /api/auth/register.</summary>
    public class AuthRegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? RoleName { get; set; }

    }

    /// <summary>Sent to POST /api/auth/logout or /api/auth/refresh.</summary>
    public class AuthRefreshRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
