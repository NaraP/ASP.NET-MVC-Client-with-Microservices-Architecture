using ECommerce.MVC.Models;

namespace ECommerce.MVC.IServices
{
    // ═══════════════════════════════════════════════════════════════════════════════
    //  IUserService — unchanged interface
    //  The rest of the MVC application (AccountController, views) depends only on
    //  this contract.  Swapping the implementation from EF Core to HttpClient
    //  requires zero changes outside this file.
    // ═══════════════════════════════════════════════════════════════════════════════
    public interface IUserService
    {
        /// <summary>
        /// Validate credentials against AuthApi.
        /// Returns the <see cref="AppUser"/> on success, <c>null</c> when the
        /// email/password is wrong or the account is inactive.
        /// </summary>
        Task<AppUser?> ValidateCredentialsAsync(string email, string password);

        /// <summary>
        /// Register a new account via AuthApi.
        /// Returns <c>(true, null, user)</c> on success or <c>(false, errorMessage, null)</c>.
        /// </summary>
        Task<(bool Success, string? Error, AppUser? User)> RegisterAsync(RegisterViewModel model);

        /// <summary>Check whether an email address is already registered.</summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Inform AuthApi that this user has just logged in.
        /// AuthApi stamps <c>LastLoginAt</c> server-side during the login call, so
        /// this is a lightweight fire-and-forget for any explicit post-login stamp.
        /// </summary>
        Task UpdateLastLoginAsync(string userId);

        /// <summary>
        /// ResetPasswordAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> ResetPasswordAsync(ResetPasswordViewModel request);
    }
}
