using StarterPack.Models.Calculator;

namespace StarterPack.Interfaces
{
    public interface ICalculatorService
    {
        CalculatorViewModel ProcessCalculation(CalculationRequest request);
        List<CalculationHistory> GetHistory();
        void ClearHistory();
    }
}
