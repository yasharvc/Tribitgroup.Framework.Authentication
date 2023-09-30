using Microsoft.AspNetCore.Mvc;
using Tribitgroup.Framewok.Identity.Shared.Interfaces;
using Tribitgroup.Framewok.Identity.Shared.Models;
using Tribitgroup.Framewok.Shared.Extensions;
using Tribitgroup.Framewok.Shared.Interfaces;

namespace Tribitgroup.Framewok.Identity.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITokenGenerator tokenGenerator;
        private readonly IScheduler scheduler;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITokenGenerator tokenGenerator, IScheduler scheduler)
        {
            _logger = logger;
            this.tokenGenerator = tokenGenerator;
            this.scheduler = scheduler;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "GetWeatherForecast")]
        public async Task<TokenInfo> TokenAsync()
        {
            return await tokenGenerator.GetTokenAsync(new UserInfo
            {
                Email = "yashar@gmail.com",
                FirstName = "yashar",
                LastName = "aliabbasi",
                Username = "yashar",
                Permissions = new[]
                {
                    new ApplicationPermission("CREATE_USER"),
                    new ApplicationPermission("DELETE_USER"),
                    new ApplicationPermission("UPDATE_USER"),
                },
                Roles = new[]
                {
                    new ApplicationRole{Id = BasicTypesExtensions.GetSequentialGuid(),Name = "Role 1" },
                    new ApplicationRole{Id = BasicTypesExtensions.GetSequentialGuid(),Name = "Role 2" },
                },
                Tenants = new[]
                {
                    new Tenant("", "T1","Tenant 1"),
                    new Tenant("", "T2","Tenant 2"),

                }
            });
        }
    }
}