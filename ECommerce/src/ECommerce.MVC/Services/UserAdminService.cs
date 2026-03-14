using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using ECommerce.MVC.Repositories;

namespace ECommerce.MVC.Services;

/// <summary>
/// Orchestrates admin user + role management.
/// Depends only on <see cref="IUserAdminRepository"/> —
/// no HTTP clients, no EF Core types leak into this layer.
/// </summary>
public class UserAdminService : IUserAdminService
{
    private readonly IUserAdminRepository _repo;

    public UserAdminService(IUserAdminRepository repo) => _repo = repo;

    // ── Queries ───────────────────────────────────────────────────────────────

    public async Task<Models.UserListViewModel> GetUserListAsync(
        string? search, string? filterRole, bool? filterActive)
    {
        var allUsers = await _repo.GetAllUsersAsync(filterActive);
        var allRoles = await _repo.GetAllRolesAsync();

        // Client-side search (name / email)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();
            allUsers = allUsers
                .Where(u => u.FullName.ToLower().Contains(q) ||
                            u.Email.ToLower().Contains(q))
                .ToList();
        }

        // Client-side role filter
        if (!string.IsNullOrWhiteSpace(filterRole))
            allUsers = allUsers
                .Where(u => u.Roles.Contains(filterRole, StringComparer.OrdinalIgnoreCase))
                .ToList();

        return new Models.UserListViewModel
        {
            Users        = allUsers,
            Roles        = allRoles,
            Search       = search,
            FilterRole   = filterRole,
            FilterActive = filterActive,
            TotalUsers   = allUsers.Count,
            ActiveUsers  = allUsers.Count(u => u.IsActive)
        };
    }

    public async Task<Models.EditUserViewModel?> GetUserForEditAsync(string userId)
    {
        var user = await _repo.GetUserByIdAsync(userId);
        if (user is null) return null;

        var allRoles = await _repo.GetAllRolesAsync();

        return new Models.EditUserViewModel
        {
            UserId         = user.Id,
            FirstName      = user.FullName.Split(' ', 2)[0],
            LastName       = user.FullName.Split(' ', 2).ElementAtOrDefault(1) ?? string.Empty,
            Email          = user.Email,
            PhoneNumber    = user.PhoneNumber,
            IsActive       = user.IsActive,
            CreatedAt      = user.CreatedAt,
            LastLoginAt    = user.LastLoginAt,
            CurrentRoles   = user.Roles,
            AvailableRoles = allRoles
        };
    }

    public async Task<Models.UserDetailsViewModel?> GetUserDetailsAsync(string userId)
    {
        var user = await _repo.GetUserByIdAsync(userId);
        if (user is null) return null;

        return new Models.UserDetailsViewModel
        {
            User  = user,
            Roles = user.Roles
        };
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public async Task<(bool Success, string? Error)> CreateUserAsync(Models.CreateUserViewModel model)
    {
        var request = new AdminCreateUserRequest
        {
            FirstName       = model.FirstName.Trim(),
            LastName        = model.LastName.Trim(),
            Email           = model.Email.Trim().ToLowerInvariant(),
            PhoneNumber     = model.PhoneNumber?.Trim(),
            Password        = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            RoleName        = model.InitialRole
        };

        var (success, error) = await _repo.CreateUserAsync(request);
        if (!success) return (false, error);

        // Optionally assign an initial role beyond the default Customer
        if (!string.IsNullOrWhiteSpace(model.InitialRole) &&
            !model.InitialRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
        {
            // Need the userId — fetch by email
            var created = (await _repo.GetAllUsersAsync())
                .FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));

            if (created is not null)
                await _repo.AssignRoleAsync(created.Id, model.InitialRole);
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateUserAsync(
        string userId, EditUserViewModel model)
    {
        var request = new UpdateUserApiRequest
        {
            FirstName   = model.FirstName.Trim(),
            LastName    = model.LastName.Trim(),
            PhoneNumber = model.PhoneNumber?.Trim(),
            IsActive    = model.IsActive
        };

        return await _repo.UpdateUserAsync(userId, request);
    }

    public Task<(bool Success, string? Error)> ToggleUserActiveAsync(string userId, bool activate) =>
        _repo.SetUserActiveAsync(userId, activate);

    // ── Role assignment ───────────────────────────────────────────────────────

    public async Task<(bool Success, string? Error)> AssignRoleAsync(string userId, string roleName)
    {
        var (success, error, _) = await _repo.AssignRoleAsync(userId, roleName);
        return (success, error);
    }

    public async Task<(bool Success, string? Error)> RemoveRoleAsync(string userId, string roleName)
    {
        var (success, error, _) = await _repo.RemoveRoleAsync(userId, roleName);
        return (success, error);
    }

    // ── Roles ─────────────────────────────────────────────────────────────────

    public async Task<RoleListViewModel> GetRoleListAsync()
    {
        var roles = await _repo.GetAllRolesAsync();
        return new RoleListViewModel { Roles = roles };
    }

    public async Task<(bool Success, string? Error)> CreateRoleAsync(CreateRoleViewModel model)
    {
        var request = new CreateRoleApiRequest
        {
            Name        = model.Name.Trim(),
            Description = model.Description.Trim()
        };

        var (success, error, _) = await _repo.CreateRoleAsync(request);
        return (success, error);
    }

    public Task<List<RoleApiDto>> GetAvailableRolesAsync() =>
        _repo.GetAllRolesAsync();
}
