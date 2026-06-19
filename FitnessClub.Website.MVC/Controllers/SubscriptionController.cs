using Microsoft.AspNetCore.Mvc;

namespace FitnessClub_Test.Website.MVC.Controllers
{
    public class SubscriptionController : Controller
    {
        public IActionResult Index()
        {
            return View("Subscription");
        }
    }
}
