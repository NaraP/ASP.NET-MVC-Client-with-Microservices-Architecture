using ECommerce.AuthApi.IServices;
using ECommerce.AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AuthApi.Controllers;

/// <summary>
/// Handles all authentication flows: register, login, token refresh, logout,
/// password change, and current-user profile retrieval.
///
/// Controller responsibility: HTTP binding, model validation, HTTP status mapping.
/// Business logic lives entirely in <see cref="IUserService"/> and <see cref="ITokenService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserService  _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService  = userService;
        _tokenService = tokenService;
    }

    // ── POST /api/auth/register ───────────────────────────────────────────────
    /// <summary>
    /// Create a new account. The Customer role is automatically assigned.
    /// Returns a ready-to-use access + refresh token pair.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var (user, error) = await _userService.RegisterAsync(request);

        if (user is null)
            return BadRequest(new ProblemDetails
            {
                Title  = "Registration failed.",
                Detail = error,
                Status = StatusCodes.Status400BadRequest
            });

        var fullUser     = await _userService.GetByIdAsync(user.Id);
        var roleNames    = fullUser?.UserRoles.Select(ur => ur.Role.Name).ToList() ?? new List<string> { "Customer" };
        var accessToken  = _tokenService.GenerateAccessToken(user, roleNames);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

        return StatusCode(StatusCodes.Status201Created, new AuthResponse
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt    = DateTime.UtcNow.AddMinutes(120),
            User = new UserDto
            {
                Id          = user.Id,
                FullName    = user.FullName,
                Email       = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles       = roleNames,
                IsActive    = user.IsActive,
                CreatedAt   = user.CreatedAt
            }
        });
    }

    // ── POST /api/auth/login ──────────────────────────────────────────────────
    /// <summary>
    /// Authenticate with email + password.
    /// Returns a JWT access token (short-lived) and a refresh token (long-lived).
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var user = await _userService.ValidateCredentialsAsync(request.Email, request.Password);

        if (user is null)
            return Unauthorized(new ProblemDetails
            {
                Title  = "Authentication failed.",
                Detail = "Invalid email or password.",
                Status = StatusCodes.Status401Unauthorized
            });

        await _userService.UpdateLastLoginAsync(user.Id);

        var roles        = user.UserRoles.Where(ur => ur.Role.IsActive).Select(ur => ur.Role.Name).ToList();
        var accessToken  = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

        return Ok(new AuthResponse
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt    = DateTime.UtcNow.AddMinutes(120),
            User = new UserDto
            {
                Id          = user.Id,
                FullName    = user.FullName,
                Email       = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles       = roles,
                IsActive    = user.IsActive,
                CreatedAt   = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            }
        });
    }

    // ── POST /api/auth/refresh ────────────────────────────────────────────────
    /// <summary>
    /// Exchange a valid refresh token for a fresh access + refresh token pair.
    /// The old refresh token is revoked immediately (rotation pattern).
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _tokenService.RefreshAsync(request.RefreshToken);

        if (result is null)
            return Unauthorized(new ProblemDetails
            {
                Title  = "Token refresh failed.",
                Detail = "Refresh token is invalid, expired, or already used.",
                Status = StatusCodes.Status401Unauthorized
            });

        return Ok(result);
    }

    // ── POST /api/auth/logout ─────────────────────────────────────────────────
    /// <summary>Revoke the supplied refresh token — single-device logout.</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
        return Ok(new { message = "Logged out successfully." });
    }

    // ── POST /api/auth/logout-all ─────────────────────────────────────────────
    /// <summary>Revoke ALL refresh tokens for the current user — all-device logout.</summary>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _tokenService.RevokeAllUserTokensAsync(userId);
        return Ok(new { message = "Logged out from all devices." });
    }

    // ── POST /api/auth/change-password ───────────────────────────────────────
    /// <summary>Change password. All existing refresh tokens are revoked.</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userId           = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var (success, error) = await _userService.ChangePasswordAsync(userId, request);

        if (!success)
            return BadRequest(new ProblemDetails
            {
                Title  = "Password change failed.",
                Detail = error,
                Status = StatusCodes.Status400BadRequest
            });

        await _tokenService.RevokeAllUserTokensAsync(userId);
        return Ok(new { message = "Password changed. Please log in again." });
    }



    // ── GET /api/auth/me ──────────────────────────────────────────────────────
    /// <summary>Return the current authenticated user's profile.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user   = await _userService.GetByIdAsync(userId);

        if (user is null)
            return NotFound(new ProblemDetails { Title = "User not found.", Status = 404 });

        var roles = user.UserRoles.Where(ur => ur.Role.IsActive).Select(ur => ur.Role.Name).ToList();

        return Ok(new UserDto
        {
            Id          = user.Id,
            FullName    = user.FullName,
            Email       = user.Email,
            PhoneNumber = user.PhoneNumber,
            Roles       = roles,
            IsActive    = user.IsActive,
            CreatedAt   = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match.");
            return ValidationProblem(ModelState);
        }

        var success = await _userService.ResetPasswordAsync(model);

        if (!success)
            return BadRequest(new ProblemDetails
            {
                Title = "Password reset failed.",
                Detail =  "Unable to reset password.",
                Status = StatusCodes.Status400BadRequest
            });

        return Ok(new
        {
            message = "Password reset successful."
        });
    }
}
