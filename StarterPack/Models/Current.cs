namespace StarterPack.Models
{
    public class Current
    {
        public int Is_Day { get; set; }
        public float Temp_C { get; set; }
        public float Wind_Kph { get; set; }
        public int Humidity { get; set; }
        public float Uv { get; set; }
        public Condition Condition { get; set; }
    }
}
