using StarterPack.Interfaces;
using System.Collections.Generic;

namespace StarterPack.Services
{
    public class WeatherIconService : IWeatherIconService
    {
        private readonly Dictionary<string, string> _conditionIcons = new()
        {
            { "sunny", "images/weather-icons/Sunny.png" },
            { "clear", "images/weather-icons/clear-night.png" },
            { "overcast", "images/weather-icons/Overcast.png" },
            { "mist", "images/weather-icons/mist.png" },
            { "sleet", "images/weather-icons/sleet.png" },
            { "freezing drizzle", "images/weather-icons/Freezingdrizzle.png" },
            { "thunder", "images/weather-icons/thunder.png" },
            { "blizzard", "images/weather-icons/blizzard.png" },
            { "light drizzle", "images/weather-icons/lightdrizzle.png" },
            { "ice pellets", "images/weather-icons/ice-pellets.png" },
            { "cloudy", "images/weather-icons/cloudy.png" },
            { "blowing snow", "images/weather-icons/Blowingsnow.png" },
            { "heavy snow", "images/weather-icons/heavysnow.png" },
            { "snow", "images/weather-icons/snow.png" },
            { "freezing fog", "images/weather-icons/freezingfog.png" },
            { "fog", "images/weather-icons/fog.png" },
            { "heavy rain", "images/weather-icons/Heavyrain.png" },
            { "rain shower", "images/weather-icons/Heavyrain.png" },
            { "freezing rain", "images/weather-icons/Freezingdrizzle.png" },
            { "rain with thunder", "images/weather-icons/thunder.png" },
            { "rain", "images/weather-icons/rain.png" }
        };

        public string GetWeatherIcon(string condition, bool isDay)
        {
            string normalizedCondition = condition.ToLower();

            if (normalizedCondition.Contains("partly cloudy"))
            {
                return isDay ? "images/weather-icons/Partlycloudyday.png" : "images/weather-icons/Partlycloudynight.png";
            }

            var priorityOrder = new[]
            {
                "blowing snow", "heavy snow", "freezing fog", "heavy rain", "rain shower",
                "freezing rain", "rain with thunder", "freezing drizzle", "light drizzle",
                "ice pellets", "thunder", "blizzard", "sleet", "snow", "fog", "rain",
                "overcast", "cloudy", "mist", "clear", "sunny"
            };

            foreach (var keyword in priorityOrder)
            {
                if (normalizedCondition.Contains(keyword))
                {
                    return _conditionIcons[keyword];
                }
            }

            // Default 
            return "images/weather-icons/Sunny.png";
        }
    }
}