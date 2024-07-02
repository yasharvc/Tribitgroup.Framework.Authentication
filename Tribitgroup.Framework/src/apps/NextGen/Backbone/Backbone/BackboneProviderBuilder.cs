using NextGen.Backbone.Backbone;
using NextGen.Backbone.Backbone.Contracts;
using NextGen.Backbone.ServiceProvider.Contracts;

namespace NextGen.Backbone.ServiceProvider
{
    public static class BackboneProviderBuilder
    {
        public static IServiceCollection AddBackboneProvider<T>(this IServiceCollection services) where T : class, IApplicationUser
        {
            services.AddSingleton<ApplicationModeProvider>();
            services.AddScoped<IBackboneProvider>(sp =>
            {
                var res = new BackboneProvider
                (
                    sp.GetRequiredService<T>(),
                    sp,
                    sp.GetRequiredService<ApplicationModeProvider>()
                );

                return res;
            });
            return services;
        }

        public static WebApplication UseBackbone(this WebApplication app)
        {
            app.MapGet("/backbone", async (IBackboneProvider backbone) =>
            {
                await backbone.OnApplicationModeChanged(ApplicationModeEnum.Stopping);
                return Results.Ok();
            });

            return app;
        }
    }
}
