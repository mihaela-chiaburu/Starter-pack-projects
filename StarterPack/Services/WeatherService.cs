using StarterPack.Interfaces;
using StarterPack.Models.Weather;
using System.Text.Json;

namespace StarterPack.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WeatherService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<WeatherDto> GetWeatherAsync(string city)
        {
            string apiKey = _config["WeatherApi"];
            string url = $"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={city}&days=1&aqi=no&alerts=no";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var weatherRaw = JsonSerializer.Deserialize<WeatherApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new WeatherDto
            {
                City = weatherRaw.Location.Name,
                Country = weatherRaw.Location.Country,
                Is_Day = weatherRaw.Current.Is_Day,
                Temp_C = weatherRaw.Current.Temp_C,
                Wind_Kph = weatherRaw.Current.Wind_Kph,
                Humidity = weatherRaw.Current.Humidity,
                Uv = weatherRaw.Current.Uv,
                Condition = weatherRaw.Current.Condition.Text,
                ConditionIcon = "https:" + weatherRaw.Current.Condition.Icon,
                Forecast = weatherRaw.Forecast.ForecastDay[0].Hour
                    .OrderBy(h => DateTime.Parse(h.Time))
                    .Where(h => DateTime.Parse(h.Time) >= DateTime.Now)
                    .Take(4)
                    .Union(
                        weatherRaw.Forecast.ForecastDay[0].Hour
                            .Where(h => DateTime.Parse(h.Time) < DateTime.Now)
                            .OrderBy(h => DateTime.Parse(h.Time))
                            .Take(4)
                    )
                    .Take(4)
                    .Select(h => new HourlyForecastDto
                    {
                        Temp_C = h.Temp_C,
                        Time = h.Time,
                        Condition = h.Condition.Text,
                        ConditionIcon = "https:" + h.Condition.Icon
                    }).ToList()
            };
        }
    }

}
