namespace StarterPack.Models
{
    public class HourlyForecastDto
    {
        public string Time { get; set; }
        public float Temp_C { get; set; }
        public string Condition { get; set; }
        public string ConditionIcon { get; set; }
    }
}
