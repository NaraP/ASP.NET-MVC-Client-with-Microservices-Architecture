using ECommerce.AuthApi.Entities;
using ECommerce.AuthApi.IServices;
using ECommerce.AuthApi.Models;
using ECommerce.AuthApi.Repository;

namespace ECommerce.AuthApi.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsActive = r.IsActive,
            UserCount = r.UserRoles?.Count ?? 0
        })
        .OrderBy(r => r.Name)
        .ToList();
    }

    public async Task<RoleDto?> GetByNameAsync(string name)
    {
        var role = await _roleRepository.GetByNameAsync(name);

        if (role == null) return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            UserCount = role.UserRoles?.Count ?? 0
        };
    }

    public async Task<(Role? Role, string? Error)> CreateAsync(CreateRoleRequest request)
    {
        var name = request.Name.Trim();

        if (await _roleRepository.ExistsByNameAsync(name))
            return (null, $"Role '{name}' already exists.");

        var role = new Role
        {
            Name = name,
            Description = request.Description?.Trim() ?? "",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _roleRepository.AddRoleAsync(role);
        await _roleRepository.SaveChangesAsync();

        return (role, null);
    }

    public async Task<(bool Success, string? Error)> AssignRoleAsync(
        string userId,
        string roleName,
        string assignedBy)
    {
        var role = await _roleRepository.GetByNameAsync(roleName);

        if (role == null || !role.IsActive)
            return (false, $"Role '{roleName}' not found or inactive.");

        var hasRole = await _roleRepository.UserHasRoleAsync(userId, role.Id);

        if (hasRole)
            return (false, $"User already has role '{roleName}'.");

        await _roleRepository.AssignRoleAsync(new UserRole
        {
            UserId = userId,
            RoleId = role.Id,
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow
        });

        await _roleRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> RemoveRoleAsync(string userId, string roleName)
    {
        var removed = await _roleRepository.RemoveRoleAsync(userId, roleName);

        if (!removed)
            return (false, $"User does not have role '{roleName}'.");

        await _roleRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var roles = await _roleRepository.GetRoleNamesForUserAsync(userId);
        return roles.ToList();
    }
}
