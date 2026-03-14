using ECommerce.AuthApi.Entities;
using ECommerce.AuthApi.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace ECommerce.AuthApi.IServices;

/// <summary>
/// User account business logic — orchestrates <see cref="IUserRepository"/>
/// and <see cref="IRoleService"/> to enforce domain rules.
/// Controllers depend on this interface, never on repositories directly.
/// </summary>
public interface IUserService
{
    Task<(AppUser? User, string? Error)> RegisterAsync(Models.RegisterRequest request);

    Task<AppUser?> ValidateCredentialsAsync(string email, string password);

    Task<bool> EmailExistsAsync(string email);

    Task UpdateLastLoginAsync(string userId);

    Task<AppUser?> GetByIdAsync(string userId);

    Task<IReadOnlyList<AppUser>> GetAllAsync(bool? isActive = null);

    Task<(bool Success, string? Error)> UpdateAsync(string userId, UpdateUserRequest request);

    Task<(bool Success, string? Error)> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    Task<(bool Success, string? Error)> SetActiveAsync(string userId, bool isActive);

    Task<IReadOnlyList<string>> GetUserRolesAsync(string userId);

    Task<bool> ResetPasswordAsync(ResetPasswordViewModel request);
}
