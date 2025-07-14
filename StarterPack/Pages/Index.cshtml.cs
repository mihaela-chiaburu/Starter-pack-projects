using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarterPack.Models.Calculator;
using StarterPack.Models.ToDo;
using StarterPack.Models.Weather;
using StarterPack.Services;

namespace StarterPack.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WeatherService _weatherService;
        private readonly CalculatorService _calculatorService;
        private readonly ToDoService _toDoService;
        private readonly WeatherIconService _weatherIconService;

        public IndexModel(
            WeatherService weatherService,
            CalculatorService calculatorService,
            ToDoService toDoService,
            WeatherIconService weatherIconService)
        {
            _weatherService = weatherService;
            _calculatorService = calculatorService;
            _toDoService = toDoService;
            _weatherIconService = weatherIconService;
        }

        // Weather Properties
        public WeatherViewModel Weather { get; set; } = WeatherViewModel.CreateDefault();
        public string SearchCity { get; set; } = "Chisinau";

        // Calculator Properties
        public CalculatorViewModel Calculator { get; set; } = new();

        // ToDo Properties
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