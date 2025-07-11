namespace StarterPack.Models.Weather
{
    public class WeatherDto
    {
        public string City { get; set; }
        public string Country { get; set; }
        public int Is_Day { get; set; }
        public float Temp_C { get; set; }
        public float Wind_Kph { get; set; }
        public int Humidity { get; set; }
        public float Uv { get; set; }
        public string Condition { get; set; }
        public string ConditionIcon { get; set; }
        public List<HourlyForecastDto> Forecast { get; set; }
    }

}
