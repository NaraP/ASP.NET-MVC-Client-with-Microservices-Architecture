using ECommerce.MVC.Models;

namespace ECommerce.MVC.Repositories;

/// <summary>
/// Persistence abstraction for admin user + role operations.
/// All data comes from <see cref="IAdminApiService"/> (HTTP → AuthApi).
/// Controllers and services depend on this interface — not on the HTTP client.
/// </summary>
public interface IUserAdminRepository
{
    // ── Users ─────────────────────────────────────────────────────────────────
    Task<List<AuthUserDto>> GetAllUsersAsync(bool? isActive = null);
    Task<AuthUserDto?> GetUserByIdAsync(string userId);
    Task<(bool Success, string? Error)> CreateUserAsync(Models.AdminCreateUserRequest request);
    Task<(bool Success, string? Error)> UpdateUserAsync(string userId, Models.UpdateUserApiRequest request);
    Task<(bool Success, string? Error)> SetUserActiveAsync(string userId, bool isActive);

    // ── Role assignment ───────────────────────────────────────────────────────
    Task<List<string>> GetUserRolesAsync(string userId);
    Task<(bool Success, string? Error, List<string> Roles)> AssignRoleAsync(string userId, string roleName);
    Task<(bool Success, string? Error, List<string> Roles)> RemoveRoleAsync(string userId, string roleName);

    // ── Roles ─────────────────────────────────────────────────────────────────
    Task<List<Models.RoleApiDto>> GetAllRolesAsync();
    Task<(bool Success, string? Error, Models.RoleApiDto? Role)> CreateRoleAsync(Models.CreateRoleApiRequest request);
}
