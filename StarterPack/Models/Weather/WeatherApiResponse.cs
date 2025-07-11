namespace StarterPack.Models.Weather
{
    public class WeatherApiResponse
    {
        public Location Location { get; set; }
        public Current Current { get; set; }
        public Forecast Forecast { get; set; }

    }
}
