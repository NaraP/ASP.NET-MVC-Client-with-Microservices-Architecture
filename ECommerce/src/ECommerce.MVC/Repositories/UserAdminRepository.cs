using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using ECommerce.MVC.Services;

namespace ECommerce.MVC.Repositories;

/// <summary>
/// Repository implementation that delegates all persistence to
/// <see cref="IAdminApiService"/> (typed HttpClient → AuthApi).
/// No EF Core, no SQL — the AuthApi owns all data.
/// </summary>
public class UserAdminRepository : IUserAdminRepository
{
    private readonly IAdminApiService _adminApi;

    public UserAdminRepository(IAdminApiService adminApi) => _adminApi = adminApi;

    // ── Users ─────────────────────────────────────────────────────────────────

    public Task<List<AuthUserDto>> GetAllUsersAsync(bool? isActive = null) =>
        _adminApi.GetUsersAsync(isActive);

    public Task<AuthUserDto?> GetUserByIdAsync(string userId) =>
        _adminApi.GetUserAsync(userId);

    public Task<(bool Success, string? Error)> CreateUserAsync(Models.AdminCreateUserRequest request) =>
        _adminApi.CreateUserAsync(request);

    public Task<(bool Success, string? Error)> UpdateUserAsync(string userId, Models.UpdateUserApiRequest request) =>
        _adminApi.UpdateUserAsync(userId, request);

    public Task<(bool Success, string? Error)> SetUserActiveAsync(string userId, bool isActive) =>
        isActive
            ? _adminApi.ActivateUserAsync(userId)
            : _adminApi.DeactivateUserAsync(userId);

    // ── Role assignment ───────────────────────────────────────────────────────

    public Task<List<string>> GetUserRolesAsync(string userId) =>
        _adminApi.GetUserRolesAsync(userId);

    public Task<(bool Success, string? Error, List<string> Roles)> AssignRoleAsync(
        string userId, string roleName) =>
        _adminApi.AssignRoleAsync(userId, roleName);

    public Task<(bool Success, string? Error, List<string> Roles)> RemoveRoleAsync(
        string userId, string roleName) =>
        _adminApi.RemoveRoleAsync(userId, roleName);

    // ── Roles ─────────────────────────────────────────────────────────────────

    public Task<List<Models.RoleApiDto>> GetAllRolesAsync() =>
        _adminApi.GetRolesAsync();

    public Task<(bool Success, string? Error, Models.RoleApiDto? Role)> CreateRoleAsync(
        Models.CreateRoleApiRequest request) =>
        _adminApi.CreateRoleAsync(request);
}
