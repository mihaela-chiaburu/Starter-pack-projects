using StarterPack.Interfaces;
using StarterPack.Models.Calculator;

namespace StarterPack.Services
{
    public class CalculatorService : ICalculatorService
    {
        private static List<CalculationHistory> _history = new();

        public CalculatorViewModel ProcessCalculation(CalculationRequest request)
        {
            var viewModel = new CalculatorViewModel
            {
                CurrentDisplay = request.CurrentDisplay,
                CurrentEquation = request.CurrentEquation,
                LastOperation = request.LastOperation,
                StoredValue = request.StoredValue,
                IsNewCalculation = request.IsNewCalculation,
                History = _history
            };

            try
            {
                switch (request.Action.ToLower())
                {
                    case "number":
                        ProcessNumber(viewModel, request.Value);
                        break;
                    case "operator":
                        ProcessOperator(viewModel, request.Value);
                        break;
                    case "equals":
                        ProcessEquals(viewModel);
                        break;
                    case "clear":
                        ProcessClear(viewModel);
                        break;
                    case "plusminus":
                        ProcessPlusMinus(viewModel);
                        break;
                    case "percent":
                        ProcessPercent(viewModel);
                        break;
                    case "decimal":
                        ProcessDecimal(viewModel);
                        break;
                    case "delete":
                        ProcessDelete(viewModel);
                        break;
                }
            }
            catch (Exception ex)
            {
                viewModel.HasError = true;
                viewModel.ErrorMessage = "Error";
                viewModel.CurrentDisplay = "Error";
            }

            return viewModel;
        }

        private void ProcessNumber(CalculatorViewModel model, string number)
        {
            if (model.IsNewCalculation || model.CurrentDisplay == "0")
            {
                model.CurrentDisplay = number;
                model.IsNewCalculation = false;
            }
            else
            {
                model.CurrentDisplay += number;
            }
        }

        private void ProcessOperator(CalculatorViewModel model, string operation)
        {
            if (!model.IsNewCalculation && !string.IsNullOrEmpty(model.LastOperation))
            {
                ProcessEquals(model);
            }

            model.StoredValue = double.Parse(model.CurrentDisplay);
            model.LastOperation = operation;
            model.IsNewCalculation = true;

            model.CurrentEquation = $"{model.StoredValue} {GetOperatorSymbol(operation)}";
        }

        private void ProcessEquals(CalculatorViewModel model)
        {
            if (string.IsNullOrEmpty(model.LastOperation)) return;

            double currentValue = double.Parse(model.CurrentDisplay);
            double result = 0;

            switch (model.LastOperation)
            {
                case "add":
                    result = model.StoredValue + currentValue;
                    break;
                case "subtract":
                    result = model.StoredValue - currentValue;
                    break;
                case "multiply":
                    result = model.StoredValue * currentValue;
                    break;
                case "divide":
                    if (currentValue == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    result = model.StoredValue / currentValue;
                    break;
            }

            // add to history
            string equation = $"{model.StoredValue} {GetOperatorSymbol(model.LastOperation)} {currentValue}";
            _history.Insert(0, new CalculationHistory
            {
                Equation = equation,
                Result = result.ToString("G15")
            });

            // only last 10 calculations
            if (_history.Count > 10)
            {
                _history = _history.Take(10).ToList();
            }

            model.CurrentDisplay = result.ToString("G15");
            model.CurrentEquation = $"{equation} =";
            model.LastOperation = "";
            model.IsNewCalculation = true;
        }

        private void ProcessClear(CalculatorViewModel model)
        {
            model.CurrentDisplay = "0";
            model.CurrentEquation = "";
            model.LastOperation = "";
            model.StoredValue = 0;
            model.IsNewCalculation = true;
            model.HasError = false;
        }

        private void ProcessPlusMinus(CalculatorViewModel model)
        {
            if (model.CurrentDisplay != "0")
            {
                if (model.CurrentDisplay.StartsWith("-"))
                {
                    model.CurrentDisplay = model.CurrentDisplay.Substring(1);
                }
                else
                {
                    model.CurrentDisplay = "-" + model.CurrentDisplay;
                }
            }
        }

        private void ProcessPercent(CalculatorViewModel model)
        {
            double value = double.Parse(model.CurrentDisplay);
            double result = value / 100;
            model.CurrentDisplay = result.ToString("G15");
        }

        private void ProcessDecimal(CalculatorViewModel model)
        {
            if (!model.CurrentDisplay.Contains("."))
            {
                model.CurrentDisplay += ".";
                model.IsNewCalculation = false;
            }
        }

        private void ProcessDelete(CalculatorViewModel model)
        {
            if (model.CurrentDisplay.Length > 1)
            {
                model.CurrentDisplay = model.CurrentDisplay.Substring(0, model.CurrentDisplay.Length - 1);
            }
            else
            {
                model.CurrentDisplay = "0";
            }
        }

        private string GetOperatorSymbol(string operation)
        {
            return operation switch
            {
                "add" => "+",
                "subtract" => "-",
                "multiply" => "×",
                "divide" => "÷",
                _ => ""
            };
        }

        public List<CalculationHistory> GetHistory()
        {
            return _history;
        }

        public void ClearHistory()
        {
            _history.Clear();
        }
    }
}
