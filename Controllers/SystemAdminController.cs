using Microsoft.AspNetCore.Mvc;

namespace FurryFriendFinder.Controllers
{
    public class SystemAdminController : Controller
    {
        public IActionResult Index()
        {
            Console.WriteLine("");
            return View();
        }
    }
}
