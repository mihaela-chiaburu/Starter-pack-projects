﻿using Microsoft.AspNetCore.Mvc;
using StarterPack.Interfaces;  

namespace StarterPack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService; 

        public WeatherController(IWeatherService weatherService)  
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