using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace ECommerce.MVC.Services
{
    // ═══════════════════════════════════════════════════════════════════════════════
    //  UserService — HttpClient implementation
    //
    //  Replaces the old EF Core / BCrypt implementation.
    //  All persistence and password logic now live in ECommerce.AuthApi.
    //  This class delegates every call to IAuthApiService and maps the
    //  AuthUserDto response back to the local AppUser model.
    //
    //  Dependency graph (same pattern as InventoryService / OrderService):
    //
    //   AccountController
    //       └─ IUserService
    //              └─ UserService
    //                     └─ IAuthApiService   ← typed HttpClient → AuthApi :5000
    // ═══════════════════════════════════════════════════════════════════════════════

    public class UserService : IUserService
    {
        private readonly IAuthApiService _authApi;

        public UserService(IAuthApiService authApi)
        {
            _authApi = authApi;
        }

        public async Task<AppUser?> ValidateCredentialsAsync(string email, string password)
        {
            var result = await _authApi.LoginAsync(email, password);

            if (result is null)
                return null;

            return MapToAppUser(result.User);
        }

        public async Task<(bool Success, string? Error, AppUser? User)> RegisterAsync(RegisterViewModel model)
        {
            var (success, error, result) = await _authApi.RegisterAsync(model);

            if (!success || result is null)
                return (false, error ?? "Registration failed.", null);

            return (true, null, MapToAppUser(result.User));
        }

        public async Task<bool> EmailExistsAsync(string email)
            => !await _authApi.IsEmailAvailableAsync(email);

        public Task UpdateLastLoginAsync(string userId)
            => Task.CompletedTask;

        public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel request)
        {
            return await _authApi.ResetPasswordAsync(request);
        }

        private static AppUser MapToAppUser(AuthUserDto dto)
        {
            return new AppUser
            {
                Id = dto.Id,
                FirstName = dto.FullName.Split(' ')[0],
                LastName = dto.FullName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Roles = dto.Roles,
                Role = dto.Roles.FirstOrDefault() ?? "Customer",
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt,
                LastLoginAt = dto.LastLoginAt
            };
        }
    }
}
