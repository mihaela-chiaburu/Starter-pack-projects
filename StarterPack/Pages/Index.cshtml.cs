using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarterPack.Models;
using StarterPack.Services;

namespace StarterPack.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WeatherService _weatherService;

        public IndexModel(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        //default values if city not found
        public string CityName { get; set; } = "Not Found, Not Found";
        public string ConditionIcon { get; set; } = "images/sun (3).png";
        public string Temperature { get; set; } = "Not Found";
        public string Condition { get; set; } = "Not Found";
        public string Wind { get; set; } = "?";
        public string Humidity { get; set; } = "?";
        public string Uv { get; set; } = "?";
        public List<ForecastViewModel> Forecast { get; set; } = new List<ForecastViewModel>
        {
            new ForecastViewModel { Icon = "images/sun (3).png", Hour = "Now", Temp = "?° C" },
            new ForecastViewModel { Icon = "images/sun (3).png", Hour = "?pm", Temp = "?° C" },
            new ForecastViewModel { Icon = "images/sun (3).png", Hour = "?pm", Temp = "?° C" },
            new ForecastViewModel { Icon = "images/sun (3).png", Hour = "?pm", Temp = "?° C" }
        };
        public string SearchCity { get; set; } = "Chisinau"; //by default

        public async Task OnGetAsync(string city = "Chisinau")
        {
            SearchCity = city;
            try
            {
                var data = await _weatherService.GetWeatherAsync(city);

                CityName = $"{data.City}, {data.Country}";
                Condition = data.Condition;
                Temperature = $"{data.Temp_C}° C";
                Wind = $"{data.Wind_Kph}km/h";
                Humidity = $"{data.Humidity}%";
                Uv = $"UV: {data.Uv}";
                ConditionIcon = GetLocalWeatherIcon(data.Condition, data.Is_Day == 1);

                Forecast = data.Forecast.Select(f =>
                {
                    var forecastHour = DateTime.Parse(f.Time).Hour;
                    var isForecastDay = forecastHour >= 6 && forecastHour < 20;
                    return new ForecastViewModel
                    {
                        Icon = GetLocalWeatherIcon(f.Condition.ToLower(), isForecastDay),
                        Hour = $"{forecastHour}h",
                        Temp = $"{f.Temp_C}° C"
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Weather fetch error: {ex.Message}");
            }
        }

        private string GetLocalWeatherIcon(string condition, bool isDay)
        {
            string iconPath = "images/weather-icons/Sunny.png";

            if (condition.Contains("sunny")) iconPath = "images/weather-icons/Sunny.png";
            else if (condition.Contains("clear")) iconPath = "images/weather-icons/clear-night.png";
            else if (condition.Contains("overcast")) iconPath = "images/weather-icons/Overcast.png";
            else if (condition.Contains("mist")) iconPath = "images/weather-icons/mist.png";
            else if (condition.Contains("sleet")) iconPath = "images/weather-icons/sleet.png";
            else if (condition.Contains("freezing drizzle")) iconPath = "images/weather-icons/freezing drizzle.png";
            else if (condition.Contains("thunder")) iconPath = "images/weather-icons/Thunder.png";
            else if (condition.Contains("blizzard")) iconPath = "images/weather-icons/blizzard.png";
            else if (condition.Contains("light drizzle")) iconPath = "images/weather-icons/light drizzle.png";
            else if (condition.Contains("ice pellets")) iconPath = "images/weather-icons/ice-pellets.png";

            if (condition.Contains("partly cloudy"))
            {
                iconPath = isDay ? "images/weather-icons/Partly cloudy day.png" : "images/weather-icons/Partly cloudy night.png";
            }
            else if (condition.Contains("cloudy"))
            {
                iconPath = "images/weather-icons/cloudy.png";
            }

            if (condition.Contains("blowing snow"))
            {
                iconPath = "images/weather-icons/Blowing snow.png";
            }
            else if (condition.Contains("heavy snow"))
            {
                iconPath = "images/weather-icons/heavy snow.png";
            }
            else if (condition.Contains("snow"))
            {
                iconPath = "images/weather-icons/snow.png";
            }

            if (condition.Contains("freezing fog"))
            {
                iconPath = "images/weather-icons/freezing fog.png";
            }
            else if (condition.Contains("fog"))
            {
                iconPath = "images/weather-icons/fog.png";
            }

            if (condition.Contains("heavy rain") || condition.Contains("rain shower"))
            {
                iconPath = "images/weather-icons/heavy rain.png";
            }
            else if (condition.Contains("freezing rain"))
            {
                iconPath = "images/weather-icons/freezing drizzle.png";
            }
            else if (condition.Contains("rain with thunder"))
            {
                iconPath = "images/weather-icons/thunder.png";
            }
            else if (condition.Contains("rain"))
            {
                iconPath = "images/weather-icons/rain.png";
            }

            return iconPath;
        }
    }
}