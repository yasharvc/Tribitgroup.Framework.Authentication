
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;
using Tribitgroup.Framewok.Identity.Shared.Models;
using Tribitgroup.Framewok.Shared.Interfaces;

namespace Tribitgroup.Framewok.Identity.API
{
    public class Program
    {

        //Add Middlewarefor dumming roles
        //Add authorized route
        //Add 
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            ConfigurationManager configuration = builder.Configuration;

            builder.Services.AddSqlServerEFForIdentity<StandardDbContext>(configuration.GetConnectionString("ConnStr") ?? throw new Exception());
            builder.AddIdentityAndJwtBearer<StandardDbContext, ApplicationUser, ApplicationRole>();


            builder.Services.AddScoped<IScheduler>(sp => new TaskDelayScheduler(sp));

            builder.InjectIdentityDependencies();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthenticationAndAutherization();

            app.Use(async (ctx, next) =>
            {
                //await Console.Out.WriteLineAsync(ctx.User.Claims.First(m=>m.Type == "test").Value);
                var identity = ctx.User.Identity;
                foreach (var claim in ctx.User.Claims)
                {
                    await Console.Out.WriteLineAsync($"{claim.Type} - {claim.Value}");
                }
                await next(ctx);
            });

            app.MapControllers();

            app.Run();
        }
    }
}