using Microsoft.AspNetCore.Mvc;
using StarterPack.Interfaces;
using StarterPack.Models.Calculator;

namespace StarterPack.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly ICalculatorService _calculatorService;

        public CalculatorController(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ProcessCalculation([FromBody] CalculationRequest request)
        {
            var result = _calculatorService.ProcessCalculation(request);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetHistory()
        {
            return Json(_calculatorService.GetHistory());
        }

        [HttpPost]
        public IActionResult ClearHistory()
        {
            _calculatorService.ClearHistory();
            return Ok();
        }
    }
}
