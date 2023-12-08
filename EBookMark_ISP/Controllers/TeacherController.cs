using Microsoft.AspNetCore.Mvc;
using EBookMark_ISP.ViewModels;
using Microsoft.EntityFrameworkCore;
using EBookMark_ISP.Models;
using Amazon.SimpleEmail.Model;
using EBookMark_ISP.Services;
using ZstdSharp.Unsafe;
using Microsoft.IdentityModel.Tokens;

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

        public IActionResult Student(int student_id)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            Student student = _context.Students.FirstOrDefault(s=>s.FkUser == student_id);
            if(student == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            string? message = HttpContext.Session.GetString("Message");
            if (message != null)
            {
                ViewBag.Message = message;
                HttpContext.Session.Remove("Message");
            }
            return View("Student", student);
        }

        public IActionResult Subjects(int student_id)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var username = HttpContext.Session.GetString("Username");
            var teacher = _context.Users.FirstOrDefault(t => t.Username == username);
            if (teacher == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var teacherSubjects = _context.Subjects.Where(ts => ts.FkTeacher == teacher.Id).ToList();
            var subjectTimePairs = teacherSubjects
                .SelectMany(ts => _context.SubjectTimes.Where(st => ts.Code == st.FkSubject))
                .Select(st => new { st.FkSchedule, st.FkSubject })
                .Distinct()
                .ToList();

            var student = _context.Students.FirstOrDefault(st => st.FkUser == student_id);
            if (student == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var studentSchedules = _context.Schedules.Where(sh => sh.FkClass == student.FkClass).ToList();

            var teacherStudentSubjects = new Dictionary<Schedule, List<Subject>>();
            foreach (var schedule in studentSchedules)
            {
                var subjectCodes = subjectTimePairs
                    .Where(p => p.FkSchedule == schedule.Id)
                    .Select(p => p.FkSubject)
                    .Distinct()
                    .ToList();

                var subjects = _context.Subjects.Where(s => subjectCodes.Contains(s.Code)).ToList();
                if (!subjects.IsNullOrEmpty())
                {
                    teacherStudentSubjects[schedule] = subjects;
                }
            }

            var viewModel = new StudentSubjectsViewModel
            {
                student = student,
                ScheduleSubjects = teacherStudentSubjects
            };

            foreach (var scheduleSubjectsPair in teacherStudentSubjects)
            {
                Schedule schedule = scheduleSubjectsPair.Key;
                List<Subject> subjects = scheduleSubjectsPair.Value;

                Console.WriteLine($"Schedule ID: {schedule.Id}");
                foreach (Subject subject in subjects)
                {
                    Console.WriteLine($"   Subject Code: {subject.Code}, Subject Name: {subject.Name}");
                }
            }

            return View(viewModel);
        }

        public IActionResult WriteMark(int student_id, int schedule_id, string subject_code)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            Student student = _context.Students.FirstOrDefault(st => st.FkUser == student_id);
            Subject subject = _context.Subjects.FirstOrDefault(sb => sb.Code == subject_code);
            var used_subject_times = _context.Marks.Where(ma => ma.FkStudent == student_id).Select(ma => ma.FkSubjectTime).ToList();
            List<SubjectTime> subjectTimes = _context.SubjectTimes.Where(st => st.FkSubject.Equals(subject_code) && st.FkSchedule == schedule_id && !used_subject_times.Contains(st.Id)).ToList();

            var viewModel = new WriteMarkViewModel
            {
                student = student,
                subject = subject,
                subjectTimes = subjectTimes
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult WriteMark(int subject_time_id, int student_id,string mark, string comment)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            try
            {
                var student_mark = new Mark
                {
                    RegistrationDate = DateTime.Now,
                    Mark1 = mark,
                    Comment = comment,
                    FkStudent = student_id,
                    FkSubjectTime = subject_time_id
                };
                _context.Marks.Add(student_mark);
                _context.SaveChanges();
                HttpContext.Session.SetString("Message", "Mark Has Been Written Successfully");
            }
            catch
            {
                HttpContext.Session.SetString("Message", "Error Occurred While Writting The Mark");
                return RedirectToAction("Student", new { student_id });
            }


            return RedirectToAction("Student", new { student_id });

        }

        [HttpGet]
        public IActionResult EditMark(int mark_id, int schedule_id, string subject_code)
        {
            if (!AccessTeacher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            Mark mark = _context.Marks.FirstOrDefault(m => m.Id == mark_id);
            var studentMarksSt = _context.Marks
                .Where(m => m.FkStudent == mark.FkStudent && m.Id != mark_id)
                .Select(m => m.FkSubjectTime)
                .ToList();

            var subjectTimes = _context.SubjectTimes
                .Where(st => st.FkSubject == subject_code && st.FkSchedule == schedule_id && !studentMarksSt.Contains(st.Id))
                .ToList();

            var viewModel = new EditMarkViewModel
            {
                subject = subject_code,
                student = _context.Students.FirstOrDefault(m => m.FkUser == mark.FkStudent),
                Mark = mark,
                SubjectTimes = subjectTimes
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
            try
            {
                var markToUpdate = _context.Marks.FirstOrDefault(m => m.Id == viewModel.Mark.Id);
                markToUpdate.Mark1 = viewModel.Mark.Mark1;
                markToUpdate.Comment = viewModel.Mark.Comment;
                markToUpdate.FkSubjectTime = viewModel.Mark.FkSubjectTime;
                markToUpdate.RegistrationDate = DateTime.Now;

                _context.SaveChanges();
                HttpContext.Session.SetString("Message", "Mark Has Been Updated Successfully");

            }
            catch
            {
                HttpContext.Session.SetString("Message", "Error Occurred While Updating The Mark");
                return RedirectToAction("GradeBook", "User", new { student_id = viewModel.Mark.FkStudent });
            }

            return RedirectToAction("GradeBook", "User", new { student_id = viewModel.Mark.FkStudent });
        }


    }
}
