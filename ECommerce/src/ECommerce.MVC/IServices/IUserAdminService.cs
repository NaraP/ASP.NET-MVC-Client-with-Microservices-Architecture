using ECommerce.MVC.Models;
using ECommerce.MVC.Services;

namespace ECommerce.MVC.IServices;

/// <summary>
/// Admin user-management business logic.
/// Controllers depend on this interface only — no repository or HTTP client types leak up.
/// </summary>
public interface IUserAdminService
{
    // ── User queries ──────────────────────────────────────────────────────────
    Task<UserListViewModel> GetUserListAsync(string? search, string? filterRole, bool? filterActive);
    Task<EditUserViewModel?> GetUserForEditAsync(string userId);
    Task<UserDetailsViewModel?> GetUserDetailsAsync(string userId);

    // ── User commands ─────────────────────────────────────────────────────────
    Task<(bool Success, string? Error)> CreateUserAsync(CreateUserViewModel model);
    Task<(bool Success, string? Error)> UpdateUserAsync(string userId, EditUserViewModel model);
    Task<(bool Success, string? Error)> ToggleUserActiveAsync(string userId, bool activate);

    // ── Role assignment ───────────────────────────────────────────────────────
    Task<(bool Success, string? Error)> AssignRoleAsync(string userId, string roleName);
    Task<(bool Success, string? Error)> RemoveRoleAsync(string userId, string roleName);

    // ── Roles ─────────────────────────────────────────────────────────────────
    Task<RoleListViewModel> GetRoleListAsync();
    Task<(bool Success, string? Error)> CreateRoleAsync(CreateRoleViewModel model);

    /// <summary>Helper — fetch all roles for populating dropdowns.</summary>
    Task<List<RoleApiDto>> GetAvailableRolesAsync();
}
