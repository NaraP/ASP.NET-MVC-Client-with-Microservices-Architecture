using ECommerce.AuthApi.Data;
using ECommerce.AuthApi.Entities;
using ECommerce.AuthApi.IServices;
using ECommerce.AuthApi.Models;
using ECommerce.AuthApi.Repository;

namespace ECommerce.AuthApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<(AppUser? User, string? Error)> RegisterAsync(Models.RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(email))
            return (null, "An account with this email address already exists.");

        var role = await _roleRepository.GetByIdAsync(request.RoleName!);

        if (role == null)
            return (null, "Default role not found.");

        var userId = GenerateUserId();

        var user = new AppUser
        {
            Id = userId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, 12),
            PhoneNumber = request.PhoneNumber ?? "",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        await _roleRepository.AssignRoleAsync(new UserRole
        {
            UserId = userId,
            RoleId = role.Id,
            AssignedBy = "registration",
            AssignedAt = DateTime.UtcNow
        });

        await _userRepository.SaveChangesAsync();

        return (user, null);
    }

    public async Task<AppUser?> ValidateCredentialsAsync(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, true);

        if (user == null || !user.IsActive)
            return null;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
            ? user
            : null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userRepository.ExistsByEmailAsync(email.Trim().ToLowerInvariant());
    }

    public async Task UpdateLastLoginAsync(string userId)
    {
        await _userRepository.UpdateLastLoginAsync(userId);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<AppUser?> GetByIdAsync(string userId)
    {
        return await _userRepository.GetByIdAsync(userId, true);
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(bool? isActive = null)
    {
        return await _userRepository.GetAllAsync(isActive);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(string userId, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return (false, "User not found");

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName.Trim();

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName.Trim();

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber.Trim();

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return (false, "User not found");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return (false, "Current password incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, 12);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> SetActiveAsync(string userId, bool isActive)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return (false, "User not found");

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<IReadOnlyList<string>> GetUserRolesAsync(string userId)
    {
        var roles = await _roleRepository.GetRoleNamesForUserAsync(userId);
        return roles.ToList().AsReadOnly();
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel request)
    {
        var user = await _userRepository.GetActiveUserByEmailAsync(request.Email);

        if (user == null)
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ConfirmPassword, 12);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    private static string GenerateUserId()
    {
        return "USR-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
    }
}

