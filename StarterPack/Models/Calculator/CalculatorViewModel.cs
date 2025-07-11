namespace StarterPack.Models.Calculator
{
    public class CalculatorViewModel
    {
        public string CurrentDisplay { get; set; } = "0";
        public string CurrentEquation { get; set; } = "";
        public List<CalculationHistory> History { get; set; } = new();
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = "";
        public string LastOperation { get; set; } = "";
        public double StoredValue { get; set; } = 0;
        public bool IsNewCalculation { get; set; } = true;
    }
}
