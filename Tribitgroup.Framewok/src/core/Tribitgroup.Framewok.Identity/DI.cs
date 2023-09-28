using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tribitgroup.Framewok.Identity.Shared.Interfaces;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public static class DI
    {
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
    }
}