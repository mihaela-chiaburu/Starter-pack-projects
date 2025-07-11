using Microsoft.AspNetCore.Mvc;

namespace StarterPack.Controllers
{
    public class ToDoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
