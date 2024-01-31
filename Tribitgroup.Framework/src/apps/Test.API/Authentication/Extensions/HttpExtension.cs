using Test.API.Authentication.Builder.Contracts;
using Test.API.Authentication.Contracts;
using Test.API.Test;

namespace Test.API.Authentication.Extensions
{
    public static class HttpExtension
    {
        public static void AddAuthentication(this WebApplication app)
        {
            app.Use(async (ctx, next) => {
                var client = new HttpClient<Tenant, Policy, Role, Permission>
                {
                    HttpContext = ctx
                };
                try
                {
                    await(await AddAuthForApp(app)).ProcessAsync(client);
                }
                catch (Exception ex)
                {
                    ctx.Response.StatusCode = 401;
                    return;
                }
                await next(ctx);
            });
        }
        private static async Task<Authenticator<Tenant, Policy, Role, Permission>> AddAuthForApp(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var builder = new AuthenticatorBuilder<Tenant, Policy, Role, Permission>(scope.ServiceProvider);
            await builder.AddPreauthenticateStepAsync<HttpCommandSetter>();
            await builder.AddPreauthenticateStepAsync<DeviceTypeSetter>();
            await builder.AddPreauthenticateStepAsync<TokenSetter>();
            await builder.AddPreauthenticateStepAsync<IPSetter>();
            await builder.AddPreauthenticateStepAsync<TokenHasher>();
            return await builder.BuildAsync();
        }
    }
}
