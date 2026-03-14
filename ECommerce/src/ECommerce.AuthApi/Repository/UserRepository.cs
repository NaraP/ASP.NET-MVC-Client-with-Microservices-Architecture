using ECommerce.AuthApi.Data;
using ECommerce.AuthApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.AuthApi.Repository;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// All DB access goes through <see cref="AuthDbContext"/> — never exposed outside this layer.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly AuthDbContext _db;

    public UserRepository(AuthDbContext db) => _db = db;

    // ── Queries ───────────────────────────────────────────────────────────────

    public async Task<AppUser?> GetByIdAsync(string userId, bool includeRoles = false)
    {
        var query = _db.Users.AsQueryable();

        if (includeRoles)
            query = query
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role);

        return await query.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<AppUser?> GetByEmailAsync(string email, bool includeRoles = false)
    {
        var normalised = email.Trim().ToLowerInvariant();
        var query      = _db.Users.AsQueryable();

        if (includeRoles)
            query = query
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role);

        return await query.FirstOrDefaultAsync(u => u.Email == normalised);
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(bool? isActive = null)
    {
        var query = _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        return await query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _db.Users.AnyAsync(u => u.Email == email.Trim().ToLowerInvariant());

    public async Task<bool> ExistsByIdAsync(string userId) =>
        await _db.Users.AnyAsync(u => u.Id == userId);

    // ── Commands ──────────────────────────────────────────────────────────────

    public async Task AddAsync(AppUser user)
    {
        await _db.Users.AddAsync(user);
    }

    public Task UpdateAsync(AppUser user)
    {
        // Entity is already tracked — just mark it modified
        _db.Entry(user).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task UpdateLastLoginAsync(string userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return;

        user.LastLoginAt = DateTime.UtcNow;
        // SaveChangesAsync is called by the service layer
    }
    public async Task<AppUser?> GetActiveUserByEmailAsync(string email)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower() && u.IsActive);
    }
    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}
