using Bussiness.Repository;
using CoreApiResponse;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRepository<Shop> _shopRepo;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger
            , IRepository<Shop> shopRepo)
        {
            _logger = logger;
            _shopRepo = shopRepo;
        }

        [HttpGet(Name = "GetWeatherForecast"), Authorize(Roles = "Admin")]
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

        [HttpGet("test")]
        public async Task<IActionResult> GetAll()
        {
            Shop data = new Shop
            {
                Name = "test",
                Address = "asdas"
            };
            await _shopRepo.InsertAsync(data);
            var result = new
            {
                Data = HttpContext.Session,
                Id = 1
            };
            return CustomResult("Success", result, HttpStatusCode.OK);
        }
    }
}