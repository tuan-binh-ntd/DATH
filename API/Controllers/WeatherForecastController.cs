using API.Dto;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API.Controllers
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
        private readonly IRepository<Demo> _demoRepository;
        private readonly IMapper _mapper;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IRepository<Demo> demoRepository,
            IMapper mapper
            )
        {
            _logger = logger;
            _demoRepository = demoRepository;
            _mapper = mapper;
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

        [HttpGet("GetDemo")]
        public async Task<Demo> GetDemo(long id)
        {
            return await _demoRepository.GetAsync(id);
        }

        [HttpDelete("Delete")]
        public async Task Delete(long id)
        {
            await _demoRepository.DeleteAsync(id);
        }

        [HttpPost("CreateOrEdit")]
        public async Task CreateOrEdit (DemoDto input)
        {
            if(input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        private async Task Create(DemoDto input)
        {
            Demo data = new Demo
            {
                Name = input.Name,
            };
            await _demoRepository.InsertAsync(data);
        }

        private async Task Update(DemoDto input)
        {
            Demo? data = await _demoRepository.GetAll().Where(e => e.Id == input.Id).FirstOrDefaultAsync();

            if(data != null)
            {
                _mapper.Map(input, data);
                //data.Name = input.Name;
                await _demoRepository.UpdateAsync(data);
            }
        }
    }
}