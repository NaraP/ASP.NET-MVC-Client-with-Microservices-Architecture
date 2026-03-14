using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.MVC.Models
{
    /// <summary>
    /// Local model representing an authenticated user.
    /// No longer an EF Core entity — populated by mapping from
    /// <see cref="ECommerce.MVC.Services.AuthUserDto"/> returned by AuthApi.
    /// </summary>
    public class AppUser
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";   // primary role (first in list)
        public List<string> Roles { get; set; } = new();        // all roles
        public bool IsActive { get; set; } = true;
        public bool EmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // ── Helpers ──────────────────────────────────────────────────────────────
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
