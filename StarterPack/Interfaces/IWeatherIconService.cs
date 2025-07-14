namespace StarterPack.Interfaces
{
    public interface IWeatherIconService
    {
        string GetWeatherIcon(string condition, bool isDay);
    }
}
