using EBookMark_ISP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace EBookMark_ISP.Controllers
{
    public class HomeWorkController : Controller
    {

        public bool AccessTeacher()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null)
            {
                return false;
            }
            if (permissions != 5)
            {
                return false;
            }
            return true;
        }
        public bool AccessStudent()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null)
            {
                return false;
            }
            if (permissions != 1)
            {
                return false;
            }
            return true;
        }



        public IActionResult Index()
        {
            return View("HomeWorkList");
        }

        public IActionResult HomeWorkList()
        {
            List<string> works = new List<string>();
            works.Add("1");
            works.Add("2");
            works.Add("3");
            return View(works);

        }
        public IActionResult HomeWork(string name)
        {

            return View("HomeWork", name);

        }

        public IActionResult AddHomeWork(string input)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }
        public IActionResult AddFile(string input)
        {
            if (!AccessStudent())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }
    }
}
