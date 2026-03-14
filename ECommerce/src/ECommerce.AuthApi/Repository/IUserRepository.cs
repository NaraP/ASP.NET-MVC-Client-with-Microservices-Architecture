using ECommerce.AuthApi.Entities;

namespace ECommerce.AuthApi.Repository;

/// <summary>
/// Abstracts all persistence operations for <see cref="AppUser"/>.
/// Services depend on this interface — never on <c>AuthDbContext</c> directly.
/// </summary>
public interface IUserRepository
{
    // ── Queries ───────────────────────────────────────────────────────────────

    /// <summary>Find a user by primary key. Returns null if not found.</summary>
    Task<AppUser?> GetByIdAsync(string userId, bool includeRoles = false);

    /// <summary>Find an active user by email address (case-insensitive).</summary>
    Task<AppUser?> GetByEmailAsync(string email, bool includeRoles = false);

    /// <summary>
    /// Return all users, optionally filtered by active state.
    /// Results are ordered by LastName, FirstName.
    /// </summary>
    Task<IReadOnlyList<AppUser>> GetAllAsync(bool? isActive = null);

    /// <summary>Return true if any user row exists with the given email.</summary>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>Return true if the user ID exists in the database.</summary>
    Task<bool> ExistsByIdAsync(string userId);

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>Persist a new user. Caller is responsible for setting all required fields.</summary>
    Task AddAsync(AppUser user);

    /// <summary>Apply pending changes to an already-tracked user entity.</summary>
    Task UpdateAsync(AppUser user);

    /// <summary>
    /// Stamp <c>LastLoginAt = UtcNow</c> for the given user ID.
    /// No-ops silently when the user is not found.
    /// </summary>
    Task UpdateLastLoginAsync(string userId);

    /// <summary>Persist all pending changes in the current unit of work.</summary>
    Task SaveChangesAsync();

    Task<AppUser?> GetActiveUserByEmailAsync(string email);
}
