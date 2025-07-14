using StarterPack.Models.Weather;

namespace StarterPack.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherDto> GetWeatherAsync(string city);
    }
}
