
using NextGen.Backbone.Backbone.Contracts;
using NextGen.Backbone.ServiceProvider;
using NextGen.Backbone.ServiceProvider.Contracts;

namespace NextGen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<Backbone.Backbone.Contracts.ILogger, ConsoleLogger>();

            builder.Services.AddScoped<IApplicationUser>(sp=> new ApplicationUser{ Name = $"Yashar {DateTime.Now.Millisecond}"});
            builder.Services.AddScoped(sp => new ApplicationUser { Name = $"Yashar {DateTime.Now.Millisecond}" });

            builder.Services.AddBackboneProvider<ApplicationUser>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseBackbone();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}
