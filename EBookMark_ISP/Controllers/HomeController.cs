using EBookMark_ISP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EBookMark_ISP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("Username");

            if(username != null) 
            {
                return RedirectToAction("Dashboard");
            }
            string errorMessage = TempData["ErrorMessage"] as string;
            if (errorMessage != null)
            {
                ViewData["ErrorMessage"] = errorMessage;
            }
            return View();
        }

        public IActionResult Dashboard()
        {
			string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if(username == null)
            {
				return RedirectToAction("Index", "Home");
			}
            ViewBag.Username = username;
            ViewBag.Permissions = permissions;
            return View();
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