using EBookMark_ISP.Models;
using EBookMark_ISP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EBookMark_ISP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EbookmarkContext _context;

        public HomeController(ILogger<HomeController> logger, EbookmarkContext context)
        {
            _logger = logger;
            _context = context;
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
        [HttpGet]
        public IActionResult Dashboard()
        {
			string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            string? message = HttpContext.Session.GetString("Message");
            if (username == null)
            {
				return RedirectToAction("Index", "Home");
			}
            if (message != null)
            {
                ViewBag.Message = message;
                HttpContext.Session.Remove("Message");
            }
            ViewBag.Username = username;
            ViewBag.Permissions = permissions;
            if(permissions == 1 ) // inicialization of filter
            {
                ViewModels.FilterViewModel viewModel = new ViewModels.FilterViewModel();
                viewModel.genders = _context.Genders.ToList();
                viewModel.scales = new List<string> { "Country", "City", "School", "Class" };


                var student_id = _context.Users.FirstOrDefault(user => user.Username == username).Id;
                var student = _context.Students.FirstOrDefault(st => st.FkUser == student_id);
                var studentSchedules = _context.Schedules.Where(sh => sh.FkClass == student.FkClass).ToList();

                var schedule_subjects = new Dictionary<Schedule, List<string>>();

                foreach (var schedule in studentSchedules)
                {
                    var subjects = _context.SubjectTimes.Where(st => st.FkSchedule == schedule.Id).Select(st => st.FkSubject).Distinct().ToList();
                    schedule_subjects[schedule] = subjects;
                }
                viewModel.schedule_subjects = schedule_subjects;
                viewModel.student = student;
                return View(viewModel);
            }

            return View(); // teacher or admin
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