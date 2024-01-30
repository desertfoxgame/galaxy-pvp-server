using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace GalaxyPvP.Helper
{
    public static class JwtAuthenticationConfigurationExtension
    {
        public static void AddJwtAutheticationConfiguration(this IServiceCollection services, JwtSettings settings)
        {
            // Register Jwt as the Authentication service
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions =>
            {
              jwtBearerOptions.TokenValidationParameters =
              new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),

                  ValidateIssuer = false,
                  //ValidIssuer = settings.Issuer,
                  ValidateAudience = false,
                  //ValidAudience = settings.Audience,

                  //ValidateLifetime = true,
                  //ClockSkew = TimeSpan.FromMinutes(settings.MinutesToExpiration)
              };
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.SecurityToken is JwtSecurityToken accessToken)
                        {
                            var userName = accessToken.Claims.FirstOrDefault(a => a.Type == Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub)?.Value;
                            var email = accessToken.Claims.FirstOrDefault(a => a.Type == "Email")?.Value;
                            context.HttpContext.Items["Id"] = userName;
                            var userInfoToken = context.HttpContext.RequestServices.GetRequiredService<UserInfoToken>();
                            userInfoToken.Id = userName;
                            userInfoToken.Email = email;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
        }
    }
}
