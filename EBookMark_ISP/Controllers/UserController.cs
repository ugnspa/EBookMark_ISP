using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace EBookMark_ISP.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            ViewBag.Username = username;
            ViewBag.Permissions = permissions;
            return View();
        }
        public bool AccessWatcher()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null)
            {
                return false;
            }
            if (!(permissions != 5 || permissions != 1))
            {
                Console.WriteLine(permissions);
                return false;
            }
            return true;
        }
        public IActionResult GradeBook(string name)
        {
            if (!AccessWatcher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            Console.WriteLine(name);
            Dictionary<string, string[]> dict = new();
            string[] marksAnglu = new string[10];
            string[] marksMatke = new string[10];
            string[] marksLietuviu = new string[10];
            marksAnglu[5] = "10";
            marksLietuviu[2] = "8";
            marksMatke[3] = "9";
            marksAnglu[9] = "7";
            dict["Lietuviu"] = marksLietuviu;
            dict["Matke"] = marksMatke;
            dict["Anglu"] = marksAnglu;
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (name == null && permissions == 1)
            {
                string username = HttpContext.Session.GetString("Username");
                ViewBag.StudentName = username;
            }
            else
            {
                ViewBag.StudentName = name;
            }
            ViewBag.Permissions = permissions;
            return View(dict);
        }

        public IActionResult ChangePassword()
        {
            string username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            string username = HttpContext.Session.GetString("Username");
            if (username == null) {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Index");
        }

        public IActionResult UserList()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            ViewBag.Permissions = permissions;
            return View();
        }
        public IActionResult Register()
        {
            string username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string email,
            string name, string surname, DateTime birthDate,
            string gender, string personalID, bool hasStudentID,
            string guardianName, string guardianSurname,
            string guardianPhoneNumber, string guardianEmail, string guardianAdress)
        {
            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Userlist");
        }

        [HttpPost]
        public IActionResult RegisterTeacher(string username, string password, string email,
            string name, string surname, DateTime employmentDate,
            string gender, string phoneNumber, string personalID)
        {
            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Userlist");
        }

        [HttpPost]
        public IActionResult RegisterAdmin(string username, string password, string email)
        {
            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Userlist");
        }

        public IActionResult ClassInfo()
        {
            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            Console.WriteLine("AAAA");
            return RedirectToAction("ClassInfo", "Class");

        }
    }                      
    
}
