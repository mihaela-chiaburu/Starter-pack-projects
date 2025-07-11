using Microsoft.AspNetCore.Mvc;
using StarterPack.Services;

namespace StarterPack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetWeather(string city)
        {
            
            var weather = await _weatherService.GetWeatherAsync(city);
            return Ok(weather);
        }

    }

}
