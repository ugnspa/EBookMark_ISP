using Microsoft.AspNetCore.Mvc;
using EBookMark_ISP.ViewModels;
using Microsoft.EntityFrameworkCore;
using EBookMark_ISP.Models;
using Amazon.SimpleEmail.Model;
using EBookMark_ISP.Services;

namespace EBookMark_ISP.Controllers
{
    public class TeacherController : Controller
    {
        private readonly EbookmarkContext _context;
        public TeacherController(EbookmarkContext context)
        {
            _context = context;
        }
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
            string username = HttpContext.Session.GetString("Username");
            var teacher = _context.Users.FirstOrDefault(t => t.Username == username);
            List<string> teacher_classes = _context.Subjects
                                          .Where(s => s.FkTeacher == teacher.Id)
                                          .SelectMany(s => s.SubjectTimes)
                                          .Select(st => st.FkSchedule)
                                          .Distinct()
                                          .Join(_context.Schedules, scheduleId => scheduleId, schedule => schedule.Id, (scheduleId, schedule) => schedule.FkClass)
                                          .Distinct()
                                          .ToList();
            List<ClassStudentsViewModel> studentsViewModels = new List<ClassStudentsViewModel>();
            foreach(string classcode in teacher_classes)
            {
                Class cl = _context.Classes.FirstOrDefault(c=> c.Code == classcode);
                if(cl != null)
                {
                    List<Student> students = _context.Students.Where(st => st.FkClass == cl.Code).ToList();
                    studentsViewModels.Add(new ClassStudentsViewModel
                    {
                        classObject= cl,
                        students = students
                    });
                }
            }

            return View(studentsViewModels);

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
