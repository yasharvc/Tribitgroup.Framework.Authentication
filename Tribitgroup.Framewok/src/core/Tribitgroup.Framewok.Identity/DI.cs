using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tribitgroup.Framewok.Identity.Middlewares;
using Tribitgroup.Framewok.Identity.Shared.Interfaces;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public static class DI
    {
        public static IServiceCollection AddSqlServerEFForIdentity<TDbContext>(this IServiceCollection services, string connectionString)
            where TDbContext : DbContext 
        {
            services.AddDbContext<TDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            

            return services;
        }

        public static IServiceCollection AddIdentityAndJwtBearer<TDbContext, TUser, TRole>(this IServiceCollection services, ConfigurationManager configuration)
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

        public static WebApplicationBuilder AddIdentityAndJwtBearer<TDbContext, TUser, TRole>(this WebApplicationBuilder builder)
            where TDbContext : DbContext
            where TUser : ApplicationUser
            where TRole : ApplicationRole
        {
            builder.Services.AddIdentityAndJwtBearer<TDbContext, TUser, TRole>(builder.Configuration);
            return builder;
        }


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
            return app;
        }

        private static void AddAuthMiddlewares(WebApplication app)
        {
            
        }

        private static void AddPreAuthenticationMiddlewares(WebApplication app)
        {
            app.UseMiddleware<TestMiddleware>();
        }
    }
}