using Microsoft.AspNetCore.Mvc;

namespace StarterPack.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
