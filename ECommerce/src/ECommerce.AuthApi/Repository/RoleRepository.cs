using ECommerce.AuthApi.Data;
using ECommerce.AuthApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.AuthApi.Repository;

/// <summary>
/// EF Core implementation of <see cref="IRoleRepository"/>.
/// </summary>
public sealed class RoleRepository : IRoleRepository
{
    private readonly AuthDbContext _db;

    public RoleRepository(AuthDbContext db) => _db = db;

    // ── Role queries ──────────────────────────────────────────────────────────

    public async Task<Role?> GetByIdAsync(string roleName) =>
        await  _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

    public async Task<Role?> GetByNameAsync(string name) =>
        await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);

    public async Task<IReadOnlyList<Role>> GetAllAsync() =>
        await _db.Roles
            .Include(r => r.UserRoles)
            .OrderBy(r => r.Name)
            .ToListAsync();

    public async Task<bool> ExistsByNameAsync(string name) =>
        await _db.Roles.AnyAsync(r => r.Name == name);

    // ── Role commands ─────────────────────────────────────────────────────────

    public async Task AddRoleAsync(Role role) =>
        await _db.Roles.AddAsync(role);

    // ── UserRole queries ──────────────────────────────────────────────────────

    public async Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(string userId) =>
        await _db.UserRoles
            .Where(ur => ur.UserId == userId && ur.Role.IsActive)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

    public async Task<bool> UserHasRoleAsync(string userId, int roleId) =>
        await _db.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

    // ── UserRole commands ─────────────────────────────────────────────────────

    public async Task AssignRoleAsync(UserRole userRole) =>
        await _db.UserRoles.AddAsync(userRole);

    public async Task<bool> RemoveRoleAsync(string userId, string roleName)
    {
        var userRole = await _db.UserRoles
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);

        if (userRole is null) return false;

        _db.UserRoles.Remove(userRole);
        return true;
    }

    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}
