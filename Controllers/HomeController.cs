using FurryFriendFinder.Models;
using FurryFriendFinder.Models.Data;
using FurryFriendFinder.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FurryFriendFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly FurryFriendFinderDbContext _context;

        public HomeController(ILogger<HomeController> logger, FurryFriendFinderDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.Publications.Include(p => p.IdUserNavigation);
            return View(new PubliComment(await _context.Publications.ToListAsync(), await _context.Comments.ToListAsync()));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}