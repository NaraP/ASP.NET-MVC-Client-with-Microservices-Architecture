using ECommerce.AuthApi.Entities;

namespace ECommerce.AuthApi.Repository;

/// <summary>
/// Abstracts all persistence operations for <see cref="Role"/> and <see cref="UserRole"/>.
/// </summary>
public interface IRoleRepository
{
    // ── Role queries ──────────────────────────────────────────────────────────

    /// <summary>Get a role by its primary key.</summary>
    Task<Role?> GetByIdAsync(string roleName);

    /// <summary>Get a role by its unique name (case-sensitive).</summary>
    Task<Role?> GetByNameAsync(string name);

    /// <summary>Return all roles ordered by name, with their user-count projection.</summary>
    Task<IReadOnlyList<Role>> GetAllAsync();

    /// <summary>Return true if a role with the given name already exists.</summary>
    Task<bool> ExistsByNameAsync(string name);

    // ── Role commands ─────────────────────────────────────────────────────────

    /// <summary>Persist a new role.</summary>
    Task AddRoleAsync(Role role);

    // ── UserRole queries ──────────────────────────────────────────────────────

    /// <summary>
    /// Return the names of all active roles currently assigned to the given user.
    /// </summary>
    Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(string userId);

    /// <summary>
    /// Return true if the user already has the specified role assigned.
    /// </summary>
    Task<bool> UserHasRoleAsync(string userId, int roleId);

    // ── UserRole commands ─────────────────────────────────────────────────────

    /// <summary>Create a new role assignment for a user.</summary>
    Task AssignRoleAsync(UserRole userRole);

    /// <summary>
    /// Remove the role assignment for the given user and role name.
    /// Returns false when the assignment did not exist.
    /// </summary>
    Task<bool> RemoveRoleAsync(string userId, string roleName);

    /// <summary>Persist all pending changes in the current unit of work.</summary>
    Task SaveChangesAsync();
}
