using ECommerce.AuthApi.Entities;
using ECommerce.AuthApi.Models;

namespace ECommerce.AuthApi.IServices;

/// <summary>
/// Role management business logic — orchestrates <see cref="IRoleRepository"/>
/// to enforce domain rules around role creation and assignment.
/// </summary>
public interface IRoleService
{
    Task<List<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByNameAsync(string name);
    Task<(Role? Role, string? Error)> CreateAsync(CreateRoleRequest request);
    Task<(bool Success, string? Error)> AssignRoleAsync(string userId, string roleName, string assignedBy);
    Task<(bool Success, string? Error)> RemoveRoleAsync(string userId, string roleName);
    Task<List<string>> GetUserRolesAsync(string userId);
}
