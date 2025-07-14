using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarterPack.Interfaces;
using StarterPack.Models.Calculator;
using StarterPack.Models.ToDo;
using StarterPack.Models.Weather;
using StarterPack.Services;

namespace StarterPack.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWeatherService _weatherService;
        private readonly ICalculatorService _calculatorService;
        private readonly IToDoService _toDoService;
        private readonly IWeatherIconService _weatherIconService;

        public IndexModel(
            IWeatherService weatherService,
            ICalculatorService calculatorService,
            IToDoService toDoService,
            IWeatherIconService weatherIconService)
        {
            _weatherService = weatherService;
            _calculatorService = calculatorService;
            _toDoService = toDoService;
            _weatherIconService = weatherIconService;
        }

        // Weather
        public WeatherViewModel Weather { get; set; } = WeatherViewModel.CreateDefault();
        public string SearchCity { get; set; } = "Chisinau";

        // Calculator 
        public CalculatorViewModel Calculator { get; set; } = new();

        // ToDo 
        public IEnumerable<ToDoItem> PendingTodos { get; set; } = new List<ToDoItem>();
        public IEnumerable<ToDoItem> CompletedTodos { get; set; } = new List<ToDoItem>();

        public async Task OnGetAsync(string city = "Chisinau")
        {
            SearchCity = city;

            await LoadWeatherDataAsync(city);
            LoadCalculatorData();
            LoadTodoData();
        }

        public async Task<IActionResult> OnPostCalculatorAsync([FromBody] CalculationRequest request)
        {
            Calculator = _calculatorService.ProcessCalculation(request);
            return new JsonResult(Calculator);
        }

        private async Task LoadWeatherDataAsync(string city)
        {
            try
            {
                var data = await _weatherService.GetWeatherAsync(city);
                Weather = WeatherViewModel.FromWeatherData(data, _weatherIconService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Weather fetch error: {ex.Message}");
                Weather = WeatherViewModel.CreateDefault();
            }
        }

        private void LoadCalculatorData()
        {
            Calculator = new CalculatorViewModel();
        }

        private void LoadTodoData()
        {
            PendingTodos = _toDoService.GetPendingTodos();
            CompletedTodos = _toDoService.GetCompletedTodos();
        }
    }
}