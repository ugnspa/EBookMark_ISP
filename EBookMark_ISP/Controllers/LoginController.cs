using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EBookMark_ISP.Models;
using EBookMark_ISP.Services;

namespace EBookMark_ISP.Controllers

{
    public class LoginController : Controller
    {

        private readonly EbookmarkContext _context;

        public LoginController(EbookmarkContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // This action handles the GET request for displaying the login form.
            return View("Index");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // This action handles the POST request when the form is submitted.
            // You can access the form data (username and password) as method parameters.

            // Perform authentication logic here.

            string passwordHash = Models.User.ComputeSha256Hash(password);
            int permissions;
            if ((permissions = IsValidUser(username, passwordHash)) != -1)
            {
                // Authentication successful, redirect to another page.
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetInt32("Permissions", permissions);
                //HttpContext.Session.SetInt32("Permissions", GetPermission(username));
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                // Authentication failed, return to the login page with an error message.
                TempData["ErrorMessage"] = "Invalid username or password";
                //Console.WriteLine(ViewData["ErrorMessage"]);
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private int IsValidUser(string username, string passwordHash)
        {
            User userToCheck = _context.Users.FirstOrDefault(u => u.Username == username);

            if (userToCheck == null)
                return -1;

            if (userToCheck.Password != passwordHash)
                return -1;

            return userToCheck.Role;
        }

        // Add your authentication logic here.
        private bool IsValidUserOffline(string username, string password)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["student"] = "student";
            dict["teacher"] = "teacher";
            dict["admin"] = "admin";

            if (!dict.ContainsKey(username))
                return false;

            if (dict[username] == password)
                return true;
            
            return false;
        }

        private int GetPermission(string username)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict["student"] = 1;
            dict["teacher"] = 5;
            dict["admin"] = 10;

            if (!dict.ContainsKey(username))
                return -1;
            else
                return dict[username];
        }
    }
}
