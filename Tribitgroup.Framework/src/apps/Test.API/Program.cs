

using Test.API.Authentication;
using Test.API.Authentication.Builder.Contracts;
using Test.API.Authentication.Contracts;
using Test.API.Test;

namespace Test.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            AddAuth(builder);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var x = await AddAuthForApp(app);

            app.Use(async (ctx, next) => { 
                var client = new HttpClient<Tenant, Policy, Role, Permission>
                {
                    HttpContext = ctx
                };
                await x.ProcessAsync(client);
                //rr.HttpCommand = Authentication.Enums.HttpCommandEnum.POST;
                await next(ctx);
            });

            app.UseAuthorization();
            app.MapPost("/", () =>
            {
                return new { controller = "Asda" };
            });

            app.MapControllers();

            app.Run();
        }

        private static async Task<Authenticator<Tenant, Policy, Role, Permission>> AddAuthForApp(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var builder = new AuthenticatorBuilder<Tenant, Policy, Role, Permission>(scope.ServiceProvider);
            await builder.AddPreauthenticateStepAsync<HttpCommanderSetter>();
            await builder.AddPreauthenticateStepAsync<DeviceTypeSetter>();
            return await builder.BuildAsync();
        }

        private static void AddAuth(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<HttpCommanderSetter>();
            builder.Services.AddScoped<DeviceTypeSetter>();
        }
    }
}
