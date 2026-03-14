using ECommerce.AuthApi.Data;
using ECommerce.AuthApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.AuthApi.Repository;

/// <summary>
/// EF Core implementation of <see cref="IRefreshTokenRepository"/>.
/// </summary>
public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthDbContext _db;

    public RefreshTokenRepository(AuthDbContext db) => _db = db;

    // ── Queries ───────────────────────────────────────────────────────────────

    public async Task<RefreshToken?> GetByTokenAsync(string token, bool includeUser = false)
    {
        var query = _db.RefreshTokens.AsQueryable();

        if (includeUser)
            query = query
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role);

        return await query.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveTokensForUserAsync(string userId) =>
        await _db.RefreshTokens
            .Where(rt => rt.UserId   == userId
                      && !rt.IsRevoked
                      && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

    // ── Commands ──────────────────────────────────────────────────────────────

    public async Task AddAsync(RefreshToken refreshToken) =>
        await _db.RefreshTokens.AddAsync(refreshToken);

    public Task UpdateAsync(RefreshToken refreshToken)
    {
        _db.Entry(refreshToken).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task RevokeAsync(string token)
    {
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (stored is null || stored.IsRevoked) return;

        stored.IsRevoked = true;
    }

    public async Task RevokeAllForUserAsync(string userId)
    {
        var activeTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var t in activeTokens)
            t.IsRevoked = true;
    }

    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}
