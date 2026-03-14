using ECommerce.MVC.Models;

namespace ECommerce.MVC.IServices
{
    /// <summary>
    /// HTTP client wrapper that calls <c>ECommerce.AuthApi</c>.
    /// Registered as a typed <see cref="HttpClient"/> — same pattern as
    /// <see cref="IInventoryService"/> and <see cref="IOrderService"/>.
    /// </summary>
    public interface IAuthApiService
    {
        // ── Authentication ────────────────────────────────────────────────────────

        /// <summary>
        /// Call POST /api/auth/login.
        /// Returns the full auth response (tokens + user) on success, null on failure.
        /// </summary>
        Task<AuthApiResponse?> LoginAsync(string email, string password);

        /// <summary>
        /// Call POST /api/auth/register.
        /// Returns (true, response) on success; (false, null) with an error message on failure.
        /// </summary>
        Task<(bool Success, string? Error, AuthApiResponse? Response)> RegisterAsync(RegisterViewModel model);

        /// <summary>
        /// Call GET /api/auth/check-email?email=…
        /// Returns true when the address is NOT yet taken (available to register).
        /// </summary>
        Task<bool> IsEmailAvailableAsync(string email);

        // ── Session management ────────────────────────────────────────────────────

        /// <summary>
        /// Call POST /api/auth/logout to revoke the stored refresh token.
        /// Silently no-ops when the token is blank.
        /// </summary>
        Task LogoutAsync(string refreshToken);

        /// <summary>
        /// ResetPasswordAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> ResetPasswordAsync(ResetPasswordViewModel request);

        /// <summary>
        /// Call POST /api/auth/refresh to rotate tokens.
        /// Returns a fresh <see cref="AuthApiResponse"/> or null when the token is invalid.
        /// </summary>
        Task<AuthApiResponse?> RefreshTokenAsync(string refreshToken);

        // ── Profile ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Call GET /api/auth/me to retrieve the current user's profile.
        /// Requires the Bearer token to already be in the session
        /// (handled by <see cref="JwtBearerHandler"/>).
        /// </summary>
        Task<AuthUserDto?> GetProfileAsync();
    }
}
