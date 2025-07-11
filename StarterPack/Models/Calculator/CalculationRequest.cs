namespace StarterPack.Models.Calculator
{
    public class CalculationRequest
    {
        public string Action { get; set; } = ""; // "number", "operator", "equals", "clear", etc.
        public string Value { get; set; } = ""; // The button value
        public string CurrentDisplay { get; set; } = "0";
        public string CurrentEquation { get; set; } = "";
        public string LastOperation { get; set; } = "";
        public double StoredValue { get; set; } = 0;
        public bool IsNewCalculation { get; set; } = true;
    }
}
