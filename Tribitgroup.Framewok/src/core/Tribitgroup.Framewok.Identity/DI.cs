using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tribitgroup.Framewok.Identity.Server;
using Tribitgroup.Framewok.Identity.Shared.DTO;
using Tribitgroup.Framewok.Identity.Shared.Interfaces;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public static class DI
    {
        public static IServiceCollection AddSqlServerEFForIdentity<TDbContext, TUser, TRole, TPermission>(this IServiceCollection services, string connectionString)
            where TDbContext : DbContext, IIdentityDbContext<TUser, TRole, TPermission>
            where TUser : ApplicationUser
            where TRole : ApplicationRole
            where TPermission : ApplicationPermission
        {
            services.AddDbContext<TDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IIdentityDbContext<TUser, TRole, TPermission>, TDbContext>();
            services.AddScoped<IIdentityServerService<TPermission>, IdentityServerService<TUser, TRole, TPermission>>();

            return services;
        }

        public static IServiceCollection AddSqlServerEFForStandardIdentity(this IServiceCollection services, string connectionString) => services.AddSqlServerEFForIdentity<StandardDbContext, ApplicationUser, ApplicationRole, ApplicationPermission>(connectionString);

        public static IServiceCollection AddIdentityAndJwtBearer<TDbContext, TUser, TRole, TPermission>(this IServiceCollection services, ConfigurationManager configuration)
            where TDbContext : DbContext
            where TUser : ApplicationUser
            where TRole : ApplicationRole
        {
            var setting = new JwtSetting();
            configuration.GetSection(nameof(JwtSetting)).Bind(setting, c => c.BindNonPublicProperties = true);


            services.AddIdentity<TUser, TRole>()
                .AddEntityFrameworkStores<TDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = setting.ValidateIssuer,
                    ValidateAudience = setting.ValidateAudience,
                    ValidateLifetime = setting.ValidateLifetime,
                    ValidateIssuerSigningKey = setting.ValidateLifetime,
                    ClockSkew = TimeSpan.Zero,

                    ValidAudience = setting.Audience,
                    ValidIssuer = setting.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.Secret))
                };
            });

            return services;
        }

        public static WebApplicationBuilder AddIdentityAndJwtBearer<TDbContext, TUser, TRole, TPermission>(this WebApplicationBuilder builder)
            where TDbContext : DbContext
            where TUser : ApplicationUser
            where TRole : ApplicationRole
            where TPermission : ApplicationPermission
        {
            builder.Services.AddIdentityAndJwtBearer<TDbContext, TUser, TRole, TPermission>(builder.Configuration);
            return builder;
        }

        public static WebApplicationBuilder AddIdentityAndJwtBearer(this WebApplicationBuilder builder) => builder.AddIdentityAndJwtBearer<StandardDbContext, ApplicationUser, ApplicationRole, ApplicationPermission>();


        public static IServiceCollection InjectIdentityDependencies(this IServiceCollection services, JwtSetting jwtSetting)
        {
            services.AddSingleton(jwtSetting);
            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            return services;
        }

        public static WebApplicationBuilder InjectIdentityDependencies(this WebApplicationBuilder builder)
        {
            var config = new JwtSetting();
            builder.Configuration.GetSection(nameof(JwtSetting)).Bind(config, c => c.BindNonPublicProperties = true);
            builder.Services.InjectIdentityDependencies(config);

            return builder;
        }

        public static WebApplication UseAuthenticationAndAutherization(this WebApplication app)
        {
            AddPreAuthenticationMiddlewares(app);
            app.UseAuthentication();
            AddAuthMiddlewares(app);
            app.UseAuthorization();
            AddPostAuthenticationMiddleware(app);

            return app;
        }

        private static void AddPostAuthenticationMiddleware(WebApplication app)
        {
            AddLoginMethod(app);
            AddPermissionMethods(app);
        }

        private static void AddPermissionMethods(WebApplication app)
        {
            app.MapPost($"{GetUrlPrefix(app)}/permission", async (
                HttpContext ctx,
                IIdentityServerService<ApplicationPermission> identityService,
                CreatePermissionInputDTO permission
                ) =>
            {
                var res = await identityService.CreatePermissionAsync(new
                    ApplicationPermission(permission.PermissionName));

                return res.FirstOrDefault();
            });

            app.MapGet($"{GetUrlPrefix(app)}/permission", (
                IIdentityServerService<ApplicationPermission> identityService) =>
            {
                return identityService.GetAllPermissionAsync();
            });
        }

        private static void AddLoginMethod(WebApplication app)
        {
            app.MapPost($"{GetUrlPrefix(app)}/login", async (HttpContext ctx,
                UserManager<ApplicationUser> userManager,
                RoleManager<ApplicationRole> roleManager,
                ITokenGenerator tokenGenerator,
                JwtSetting jwtSetting,
                StandardDbContext identityDbContext,
                IIdentityServerService<ApplicationPermission> identityServerService
                    , LoginWithUsernamePasswordInputDTO model) =>
            {
                var username = model.Username ?? "";
                var password = model.Password ?? "";
                var user = await userManager.FindByNameAsync(username);
                if (user != null && await userManager.CheckPasswordAsync(user, password))
                {
                    var userRoles = await userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }


                    var token = CreateToken(authClaims, jwtSetting);
                    var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
                    var refreshToken = GenerateRefreshToken();

                    identityDbContext.RefreshTokens.Add(new UserRefreshToken
                    {
                        RefreshToken = refreshToken,
                        Token = tokenStr,
                        UsedCount = 0,
                        ApplicationUserId = user.Id,
                        ValidUntil = DateTime.UtcNow.AddMinutes(jwtSetting.RefreshTokenExpiresInMinutes),
                    });

                    await userManager.UpdateAsync(user);

                    await identityDbContext.SaveChangesAsync();

                    return new
                    {
                        Token = tokenStr,
                        RefreshToken = refreshToken,
                        Expiration = token.ValidTo
                    };
                }

                throw new UnauthorizedAccessException();
            });
        }

        private static object GetUrlPrefix(WebApplication app)
        {
            return "";
        }

        private static void AddAuthMiddlewares(WebApplication app)
        {
            
        }

        private static void AddPreAuthenticationMiddlewares(WebApplication app)
        {
            //app.UseMiddleware<ClientAuthenticatorMiddleware>();
        }

        static JwtSecurityToken CreateToken(List<Claim> authClaims, JwtSetting _jwtSetting)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Secret));
            uint tokenValidityInSeconds = _jwtSetting.ExpiresInSeconds;

            var token = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                expires: DateTime.Now.AddSeconds(tokenValidityInSeconds),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        static ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token, JwtSetting jwtSetting)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
    }
}