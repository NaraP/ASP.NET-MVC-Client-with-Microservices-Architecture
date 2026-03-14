using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ECommerce.AuthApi;

public static class JwtExtensions
{
    /// <summary>Validate incoming Bearer tokens — AuthApi uses this for its own protected endpoints.</summary>
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var key      = config["Jwt:Key"]!;
        var issuer   = config["Jwt:Issuer"]!;
        var audience = config["Jwt:Audience"]!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer           = true,
                    ValidIssuer              = issuer,
                    ValidateAudience         = true,
                    ValidAudience            = audience,
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }

    /// <summary>Configure Swagger UI with "Authorize" button so you can test protected endpoints.</summary>
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "TechStore — Auth API",
                Version     = "v1",
                Description = "Central Authorization Service. Issues JWT access tokens and refresh tokens used by Inventory, Order and Payment APIs."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "Bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Paste your JWT access token here."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
