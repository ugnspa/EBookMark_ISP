using Microsoft.AspNetCore.Mvc;

namespace EBookMark_ISP.Controllers
{
    public class ClassController : Controller
    {
        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            ViewBag.Permissions = permissions;
            return View();
        }
        
        public IActionResult ClassInfo()
        {
            Console.WriteLine("AAAA");
            //string username = HttpContext.Session.GetString("Username");
            //if (username == null)
            //{
            //    return RedirectToAction("Dashboard", "Home");
            //}
            return View();
        }

        [HttpGet]
        public IActionResult ClassInfo(string code)
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }
        public IActionResult Create() 
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name, int year)
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Index", "Class");
        }

        [HttpGet]
        public IActionResult Remove(string code) 
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Index", "Class");
        }

        public IActionResult Modify()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Modify(string code)
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Modify(string name, int studentCount, int year, string code)
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Index", "Class");
        }

        public IActionResult AddStudentToClass()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Modify", "Class");
        }

        [HttpGet]
        public IActionResult AddStudentToClass(string id)
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Modify", "Class");
        }
        //[HttpPost]
        //public IActionResult AddStudentToClass(string id)
        //{
        //    string username = HttpContext.Session.GetString("Username");
        //    int? permissions = HttpContext.Session.GetInt32("Permissions");
        //    if (username == null || permissions == null || permissions < 9)
        //    {
        //        return RedirectToAction("Dashboard", "Home");
        //    }
        //    return RedirectToAction("Modify", "Class");
        //}
    }
}
