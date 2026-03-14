using ECommerce.AuthApi.Data;
using ECommerce.AuthApi.Entities;
using ECommerce.AuthApi.IServices;
using ECommerce.AuthApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.AuthApi.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly AuthDbContext  _db;

    public TokenService(IConfiguration config, AuthDbContext db)
    {
        _config = config;
        _db     = db;
    }

    // ── Access token ─────────────────────────────────────────────────────────
    public string GenerateAccessToken(AppUser user, IEnumerable<string> roles)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var minutes = _config.GetValue<int>("Jwt:AccessTokenMinutes", 120);

        // Build claims — all roles added as separate Role claims so [Authorize(Roles="Admin")]
        // and User.IsInRole() work out of the box on every consumer API.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name,  user.FullName),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new("firstName", user.FirstName),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ── Refresh token ─────────────────────────────────────────────────────────
    public async Task<string> GenerateRefreshTokenAsync(string userId)
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var days = _config.GetValue<int>("Jwt:RefreshTokenDays", 7);

        _db.RefreshTokens.Add(new RefreshToken
        {
            Token     = raw,
            UserId    = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(days),
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return raw;
    }

    // ── Rotate refresh token ──────────────────────────────────────────────────
    public async Task<AuthResponse?> RefreshAsync(string refreshToken)
    {
        var stored = await _db.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow)
            return null;

        if (!stored.User.IsActive)
            return null;

        var roles = stored.User.UserRoles
            .Where(ur => ur.Role.IsActive)
            .Select(ur => ur.Role.Name)
            .ToList();

        // Rotate: revoke old, issue new
        var newRaw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var days   = _config.GetValue<int>("Jwt:RefreshTokenDays", 7);
        var expiresAt = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenMinutes", 120));

        stored.IsRevoked       = true;
        stored.ReplacedByToken = newRaw;

        _db.RefreshTokens.Add(new RefreshToken
        {
            Token     = newRaw,
            UserId    = stored.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(days)
        });

        stored.User.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken  = GenerateAccessToken(stored.User, roles),
            RefreshToken = newRaw,
            ExpiresAt    = expiresAt,
            User         = MapUser(stored.User, roles)
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (stored is null || stored.IsRevoked) return;
        stored.IsRevoked = true;
        await _db.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(string userId)
    {
        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var t in tokens) t.IsRevoked = true;
        await _db.SaveChangesAsync();
    }

    // ── helper ────────────────────────────────────────────────────────────────
    private static UserDto MapUser(AppUser u, List<string> roles) => new()
    {
        Id          = u.Id,
        FullName    = u.FullName,
        Email       = u.Email,
        PhoneNumber = u.PhoneNumber,
        Roles       = roles,
        IsActive    = u.IsActive,
        CreatedAt   = u.CreatedAt,
        LastLoginAt = u.LastLoginAt
    };
}
