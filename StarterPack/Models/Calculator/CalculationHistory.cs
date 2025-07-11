namespace StarterPack.Models.Calculator
{
    public class CalculationHistory
    {
        public string Equation { get; set; } = "";
        public string Result { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
