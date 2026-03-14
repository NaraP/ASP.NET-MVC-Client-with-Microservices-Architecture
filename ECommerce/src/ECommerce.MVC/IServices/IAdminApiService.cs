using ECommerce.MVC.Models;

namespace ECommerce.MVC.IServices
{
    /// <summary>
    /// Admin-only HTTP calls to ECommerce.AuthApi.
    /// All endpoints require a valid Admin JWT (injected by JwtBearerHandler).
    /// Registered as a typed HttpClient in Program.cs — same pattern as IInventoryService.
    /// </summary>
    public interface IAdminApiService
    {
        // ── Users ─────────────────────────────────────────────────────────────────
        Task<List<AuthUserDto>> GetUsersAsync(bool? isActive = null);
        Task<AuthUserDto?> GetUserAsync(string userId);
        Task<(bool Success, string? Error)> CreateUserAsync(AdminCreateUserRequest request);
        Task<(bool Success, string? Error)> UpdateUserAsync(string userId, UpdateUserApiRequest request);
        Task<(bool Success, string? Error)> ActivateUserAsync(string userId);
        Task<(bool Success, string? Error)> DeactivateUserAsync(string userId);

        // ── Role assignment ───────────────────────────────────────────────────────
        Task<(bool Success, string? Error, List<string> Roles)> AssignRoleAsync(string userId, string roleName);
        Task<(bool Success, string? Error, List<string> Roles)> RemoveRoleAsync(string userId, string roleName);
        Task<List<string>> GetUserRolesAsync(string userId);

        // ── Roles ─────────────────────────────────────────────────────────────────
        Task<List<RoleApiDto>> GetRolesAsync();
        Task<(bool Success, string? Error, RoleApiDto? Role)> CreateRoleAsync(CreateRoleApiRequest request);
    }
}
