using System.ComponentModel.DataAnnotations;

namespace ECommerce.MVC.Models
{
    // ── User list / index ─────────────────────────────────────────────────────────

    public class UserListViewModel
    {
        public List<AuthUserDto> Users { get; set; } = new();
        public List<RoleApiDto> Roles { get; set; } = new();
        public string? FilterRole { get; set; }
        public bool? FilterActive { get; set; }
        public string? Search { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
    }

    // ── Create user ───────────────────────────────────────────────────────────────

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, and a digit.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>Optional role to assign immediately after creation.</summary>
        [Display(Name = "Initial Role")]
        public string? InitialRole { get; set; }

        /// <summary>Populated from AuthApi for the role dropdown.</summary>
        public List<RoleApiDto> AvailableRoles { get; set; } = new();
    }

    // ── Edit user ─────────────────────────────────────────────────────────────────

    public class EditUserViewModel
    {
        public string UserId { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Phone, MaxLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        // ── Read-only display ─────────────────────────────────────────────────────
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // ── Role management ───────────────────────────────────────────────────────
        public List<string> CurrentRoles { get; set; } = new();
        public List<RoleApiDto> AvailableRoles { get; set; } = new();

        /// <summary>Role name to assign — bound from the Assign Role form.</summary>
        [Display(Name = "Role to Assign")]
        public string? RoleToAssign { get; set; }
    }

    // ── User details ──────────────────────────────────────────────────────────────

    public class UserDetailsViewModel
    {
        public AuthUserDto User { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }

    // ── Roles index ───────────────────────────────────────────────────────────────

    public class RoleListViewModel
    {
        public List<RoleApiDto> Roles { get; set; } = new();
    }

    // ── Create role ───────────────────────────────────────────────────────────────

    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role name is required.")]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]*$",
            ErrorMessage = "Role name must start with a letter and contain only letters, digits, or underscores.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
    }

    // ── Assign / remove role (AJAX / form post) ───────────────────────────────────

    public class RoleActionViewModel
    {
        [Required] public string UserId { get; set; } = string.Empty;
        [Required] public string RoleName { get; set; } = string.Empty;
    }
}
