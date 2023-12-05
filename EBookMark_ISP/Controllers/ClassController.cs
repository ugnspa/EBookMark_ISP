using Microsoft.AspNetCore.Mvc;
using EBookMark_ISP.Models;
using Microsoft.EntityFrameworkCore;

namespace EBookMark_ISP.Controllers
{
    public class ClassController : Controller
    {

        private readonly EbookmarkContext _context;

        public ClassController(EbookmarkContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            ViewBag.Permissions = permissions;
            ViewBag.Classes = _context.Classes.ToList();
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


            if(permissions == 1)
            {
                Console.WriteLine("HIIIIIIIIIIIIIIIIIIIIIIII");
                code = _context.Students.Include(s => s.FkUserNavigation).
                    FirstOrDefault(s => s.FkUserNavigation.Username == username).FkClass;
                Console.WriteLine(code);
            }

            Class filteredClass = _context.Classes.Where(c => c.Code == code).FirstOrDefault();

            List<Student> students = _context.Students.Where(s => s.FkClass == code).ToList();

            ViewBag.Class = filteredClass;
            ViewBag.Students = students;

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

            string code = GenerateClassCode(name, year);
            Class newClass = new Class
            {
                Code = code,
                Name = name,
                StudentsCount = 0,
                Year = year
            };

            _context.Classes.Add(newClass);
            _context.SaveChanges();
            
            return RedirectToAction("Index", "Class");
        }

        private string GenerateClassCode(string name, int year)
        {
            string code = year.ToString() + name + DateTime.Now.Year.ToString();
            return code;
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

            Console.WriteLine("Remove:");
            Console.WriteLine($"Code {code}");

            List<Student> classStudents = _context.Students.Where(s => s.FkClass == code).ToList();

            foreach(var student in classStudents)
            {
                student.FkClass = null;
            }

            Class classToRemove = _context.Classes.FirstOrDefault(c => c.Code == code);
            if (classToRemove != null)
            {
                _context.Classes.Remove(classToRemove);
            }

            _context.SaveChanges();

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


            Class filteredClass = _context.Classes.Where(c => c.Code == code).FirstOrDefault();

            List<Student> classStudents = _context.Students.Where(s => s.FkClass == code).ToList();

            List<Student> studentsToAdd = _context.Students.Where(s => s.FkClass == null).ToList();

            ViewBag.Class = filteredClass;
            ViewBag.Students = classStudents;
            ViewBag.StudentsToAdd = studentsToAdd;

            return View();
        }

        [HttpPost]
        public IActionResult Modify(string name, int studentCount, int year, string code)
        {
            Console.WriteLine("Modify post:");
            Console.WriteLine($"Name {name}");
            Console.WriteLine($"Count {studentCount}");
            Console.WriteLine($"Year {year}");
            Console.WriteLine($"Code {code}");

            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            Class filteredClass = _context.Classes.FirstOrDefault(c => c.Code == code);

            if(filteredClass != null)
            {
                filteredClass.Name = name;
                filteredClass.StudentsCount = studentCount;
                filteredClass.Year = year;
                _context.SaveChanges();
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
        public IActionResult AddStudentToClass(string classCode, int studentId)
        {
            Console.WriteLine($"class code: {classCode}");
            Console.WriteLine($"student ID: {studentId}");
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            Student student = _context.Students.FirstOrDefault(s => s.FkUser == studentId);
            if(student != null)
            {
                _context.Classes.FirstOrDefault(c => c.Code == classCode).StudentsCount++;
                student.FkClass = classCode;
                _context.SaveChanges();
            }

            return RedirectToAction("Modify", "Class", new {code = classCode});
        }

        [HttpGet]
        public IActionResult RemoveStudentFromClass(string classCode, int studentId)
        {
            
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            Console.WriteLine("Remove student from class:");
            Console.WriteLine($" class code: {classCode}");
            Console.WriteLine($" student ID: {studentId}");

            Student student = _context.Students.FirstOrDefault(s => s.FkUser == studentId);
            if (student != null)
            {
                _context.Classes.FirstOrDefault(c => c.Code == classCode).StudentsCount--;
                student.FkClass = null;
                _context.SaveChanges();
            }

            return RedirectToAction("Modify", "Class", new { code = classCode });
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
