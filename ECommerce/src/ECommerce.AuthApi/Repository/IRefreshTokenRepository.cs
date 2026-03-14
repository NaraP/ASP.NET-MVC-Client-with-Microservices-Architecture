using ECommerce.AuthApi.Entities;

namespace ECommerce.AuthApi.Repository;

/// <summary>
/// Abstracts all persistence operations for <see cref="RefreshToken"/>.
/// </summary>
public interface IRefreshTokenRepository
{
    // ── Queries ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Find a refresh token by its raw token string.
    /// Eagerly loads the <c>User</c> and their <c>UserRoles → Role</c> when
    /// <paramref name="includeUser"/> is true.
    /// </summary>
    Task<RefreshToken?> GetByTokenAsync(string token, bool includeUser = false);

    /// <summary>
    /// Return all non-revoked, non-expired tokens for the given user.
    /// </summary>
    Task<IReadOnlyList<RefreshToken>> GetActiveTokensForUserAsync(string userId);

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>Persist a new refresh token.</summary>
    Task AddAsync(RefreshToken refreshToken);

    /// <summary>Apply pending changes to an already-tracked token entity.</summary>
    Task UpdateAsync(RefreshToken refreshToken);

    /// <summary>
    /// Revoke a single token identified by its raw value.
    /// No-ops silently when the token is not found or already revoked.
    /// </summary>
    Task RevokeAsync(string token);

    /// <summary>
    /// Revoke ALL active refresh tokens for a given user (logout-all-devices).
    /// </summary>
    Task RevokeAllForUserAsync(string userId);

    /// <summary>Persist all pending changes in the current unit of work.</summary>
    Task SaveChangesAsync();
}
