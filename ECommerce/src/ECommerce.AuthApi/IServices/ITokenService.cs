using ECommerce.AuthApi.Entities;
using ECommerce.AuthApi.Models;

namespace ECommerce.AuthApi.IServices;

/// <summary>
/// JWT access-token generation and refresh-token lifecycle management.
/// </summary>
public interface ITokenService
{
    /// <summary>Build a signed JWT access token carrying the user's claims and roles.</summary>
    string GenerateAccessToken(AppUser user, IEnumerable<string> roles);

    /// <summary>Create a cryptographically-random refresh token, persist it, and return the raw value.</summary>
    Task<string> GenerateRefreshTokenAsync(string userId);

    /// <summary>
    /// Validate a refresh token, rotate it (issue new one, revoke old one), and return
    /// a fresh AuthResponse.  Returns null when the token is invalid or expired.
    /// </summary>
    Task<AuthResponse?> RefreshAsync(string refreshToken);

    /// <summary>Revoke a single refresh token (logout).</summary>
    Task RevokeRefreshTokenAsync(string refreshToken);

    /// <summary>Revoke all refresh tokens for a user (logout-all-devices).</summary>
    Task RevokeAllUserTokensAsync(string userId);
}
