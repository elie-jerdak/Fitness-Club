using Microsoft.AspNetCore.Mvc;

namespace YourProjectName.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Classes()
        {
            return View();
        }

        public IActionResult Private()
        {
            return View();
        }
        public IActionResult Premade()
        {
            return View();
        }


    }
}
