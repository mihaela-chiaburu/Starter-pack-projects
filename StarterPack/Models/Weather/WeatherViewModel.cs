using StarterPack.Interfaces;
using StarterPack.Models.Weather;
using StarterPack.Services;

namespace StarterPack.Models.Weather
{
    public class WeatherViewModel
    {
        public string CityName { get; set; } = "Not Found, Not Found";
        public string ConditionIcon { get; set; } = "images/sun (3).png";
        public string Temperature { get; set; } = "Not Found";
        public string Condition { get; set; } = "Not Found";
        public string Wind { get; set; } = "?";
        public string Humidity { get; set; } = "?";
        public string Uv { get; set; } = "?";
        public List<ForecastViewModel> Forecast { get; set; } = new();

        public static WeatherViewModel CreateDefault()
        {
            return new WeatherViewModel
            {
                Forecast = new List<ForecastViewModel>
                {
                    new ForecastViewModel { Icon = "images/sun (3).png", Hour = "Now", Temp = "?° C" },
                    new ForecastViewModel { Icon = "images/sun (3).png", Hour = "?pm", Temp = "?° C" },
                    new ForecastViewModel { Icon = "images/sun (3).png", Hour = "?pm", Temp = "?° C" },
                    new ForecastViewModel { Icon = "images/sun (3).png", Hour = "?pm", Temp = "?° C" }
                }
            };
        }

        public static WeatherViewModel FromWeatherData(WeatherDto data, IWeatherIconService iconService)
        {
            return new WeatherViewModel
            {
                CityName = $"{data.City}, {data.Country}",
                Condition = data.Condition,
                Temperature = $"{data.Temp_C}° C",
                Wind = $"{data.Wind_Kph}km/h",
                Humidity = $"{data.Humidity}%",
                Uv = $"UV: {data.Uv}",
                ConditionIcon = iconService.GetWeatherIcon(data.Condition, data.Is_Day == 1),
                Forecast = data.Forecast.Select(f =>
                {
                    var forecastHour = DateTime.Parse(f.Time).Hour;
                    var isForecastDay = forecastHour >= 6 && forecastHour < 20;
                    return new ForecastViewModel
                    {
                        Icon = iconService.GetWeatherIcon(f.Condition.ToLower(), isForecastDay),
                        Hour = $"{forecastHour}h",
                        Temp = $"{f.Temp_C}° C"
                    };
                }).ToList()
            };
        }
    }
}