using Microsoft.AspNetCore.Mvc;
using EBookMark_ISP.ViewModels;

namespace EBookMark_ISP.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Home");
        }

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
        public IActionResult AllStudents()
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            List<string> students = new List<string>();
            students.Add("student1");
            students.Add("student2");
            students.Add("student3");
            students.Add("student4");
            return View(students);

        }

        public IActionResult Student(string name)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View("Student", name);
        }

        public IActionResult Subjects(string name)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            List<string> subjects = new List<string>();
            subjects.Add(name);
            subjects.Add("Matke");
            subjects.Add("Anglu");
            subjects.Add("Lietuviu");
            return View(subjects);
        }
        public IActionResult WriteMark(string subject, string name)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            List<string> strings = new List<string>();
            strings.Add(name);
            strings.Add(subject);
            return View(strings);
        }
        [HttpPost]
        public IActionResult WriteMark(string subject, string name, string mark)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Student", new { name });

        }

        [HttpGet]
        public IActionResult EditMark(string name, string mark, string subject, string day)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var viewModel = new EditMarkViewModel
            {
                Name = name,
                Subject = subject,
                Day = day,
                Mark = mark
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditMark(EditMarkViewModel viewModel)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("GradeBook", "User", new { name = viewModel.Name });
            }

            return RedirectToAction("Dashboard", "Home");
        }


    }
}
