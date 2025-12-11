using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Shared.Extensions;

/// <summary>
/// Shared JWT Bearer authentication configuration for microservices.
/// All services use the same JWT secret, issuer, and audience to validate tokens.
/// </summary>
public static class JwtAuthenticationExtensions
{

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<JwtBearerEvents>? configureEvents = null)
    {
        // Get JWT configuration - same keys as GenerateJwtToken uses
        var jwtKey = configuration["Jwt:Key"];
        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];

        // Validate that required settings are configured
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("Jwt:Key is not configured. Set it via user secrets or appsettings.json");
        if (string.IsNullOrEmpty(jwtIssuer))
            throw new InvalidOperationException("Jwt:Issuer is not configured");
        if (string.IsNullOrEmpty(jwtAudience))
            throw new InvalidOperationException("Jwt:Audience is not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero, // Remove default 5 minute tolerance
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };

            // Allow custom event configuration
            if (configureEvents != null)
            {
                options.Events = new JwtBearerEvents();
                configureEvents(options.Events);
            }
        });

        services.AddAuthorization();
        return services;
    }
}