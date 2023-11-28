using EBookMark_ISP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.DependencyResolver;
using Org.BouncyCastle.Bcpg;
using System.Text;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace EBookMark_ISP.Controllers
{
    public class UserController : Controller
    {

        private readonly EbookmarkContext _context;

        public UserController(EbookmarkContext context)
        {
            _context = context;
        }
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

            var genders = _context.Genders.ToList();
            ViewBag.GenderOptions = new SelectList(genders, "Id", "Name");

            var schools = _context.Schools.ToList();
            ViewBag.SchoolOptions = new SelectList(schools, "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email,
            string name, string surname, DateTime birthDate,
            string gender, string personalID, bool hasStudentID,
            string guardianName, string guardianSurname,
            string guardianPhoneNumber, string guardianEmail, string guardianAdress, int school)
        {
            //Console.WriteLine($"Username: {username}");
            //Console.WriteLine($"Password: {password}");
            //Console.WriteLine($"Email: {email}");
            //Console.WriteLine($"Name: {name}");
            //Console.WriteLine($"Surname: {surname}");
            //Console.WriteLine($"BirthDate: {birthDate}");
            //Console.WriteLine($"Gender: {gender}");
            //Console.WriteLine($"PersonalID: {personalID}");
            //Console.WriteLine($"HasStudentID: {hasStudentID}");
            //Console.WriteLine($"GuardianName: {guardianName}");
            //Console.WriteLine($"GuardianSurname: {guardianSurname}");
            //Console.WriteLine($"GuardianPhoneNumber: {guardianPhoneNumber}");
            //Console.WriteLine($"GuardianEmail: {guardianEmail}");
            //Console.WriteLine($"GuardianAddress: {guardianAdress}");
            //Console.WriteLine($"TempPassword: {tempPassword}");

            string tempPassword = GenerateTemporaryPassword();
            int userId = RegisterUser(username, tempPassword, email, 1);
            int guardianId = RegisterGuardian(guardianName, guardianSurname, guardianPhoneNumber,
                guardianEmail, guardianAdress);
            var student = new Student
            {
                PersonalCode = personalID,
                Name = name,
                Surname = surname,
                BirthDate = birthDate,
                Document = hasStudentID,
                Gender = int.Parse(gender), // Assuming gender is an integer ID
                FkUser = userId, // Use the generated User ID
                FkSchool = school, // Replace with the correct school ID
                FkGuardian = guardianId // Replace with the correct guardian ID
            };

            _context.Students.Add(student);
            GetGuardianById(guardianId).Students.Add(student);
            _context.SaveChanges();

            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Userlist");
        }

        [HttpPost]
        public IActionResult RegisterTeacher(string username, string email,
            string name, string surname, DateTime employmentDate,
            int gender, string phoneNumber, string personalID, int school)
        {

            string tempPassword = GenerateTemporaryPassword();


            //Console.WriteLine($"Username: {username}");
            //Console.WriteLine($"Email: {email}");
            //Console.WriteLine($"Name: {name}");
            //Console.WriteLine($"Surname: {surname}");
            //Console.WriteLine($"EmploymentDate: {employmentDate}");
            //Console.WriteLine($"Gender: {gender}");
            //Console.WriteLine($"PhoneNumber: {phoneNumber}");
            //Console.WriteLine($"PersonalID: {personalID}");
            //Console.WriteLine($"School: {school}");
            //Console.WriteLine($"Password: {tempPassword}");

            int userId = RegisterUser(username, tempPassword, email, 5);

            Teacher teacher = new Teacher
            {
                PersonalCode = personalID,
                Name = name,
                Surname = surname,
                EmploymentDate = employmentDate,
                PhoneNumber = phoneNumber,
                Gender = gender,
                FkUser = userId
            };

            _context.Teachers.Add(teacher);
            _context.SaveChanges();
            SchoolTeacher schoolTeacher = new SchoolTeacher 
            { 
                FkTeacher = userId, 
                FkSchool = school 
            };
            _context.SchoolTeachers.Add(schoolTeacher);
            _context.SaveChanges();

            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Userlist");
        }

        [HttpPost]
        public IActionResult RegisterAdmin(string username, string email)
        {
            string tempPassword = GenerateTemporaryPassword();
            //Console.WriteLine($"Username: {username}");
            //Console.WriteLine($"Email: {email}");

            int userId = RegisterUser(username, tempPassword, email, 10);

            Admin admin = new Admin { FkUser = userId };
            _context.Admins.Add(admin);
            _context.SaveChanges();

            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Userlist");
        }

        private void RegisterDefaultAccounts()
        {
            RegisterDefaultStudent();
            RegisterDefaultTeacher();
            RegisterDefaultAdmin();
        }

        private void RegisterDefaultStudent()
        {
            int userId = RegisterUser("student", "student", "student@email.com", 1);

            var student = new Student
            {
                PersonalCode = "0000000000",
                Name = "Student",
                Surname = "Student",
                BirthDate = DateTime.Now,
                Document = true,
                Gender = 1, // Assuming gender is an integer ID
                FkUser = userId, // Use the generated User ID
                FkSchool = 1, // Replace with the correct school ID
                FkGuardian = 1 // Replace with the correct guardian ID
            };

            _context.Students.Add(student);
            _context.SaveChanges();
        }
        private void RegisterDefaultTeacher()
        {
            int userId = RegisterUser("teacher", "teacher", "teacher@email.com", 5);

            Teacher teacher = new Teacher
            {
                PersonalCode = "0000000000",
                Name = "Teacher",
                Surname = "Teacher",
                EmploymentDate = DateTime.Now,
                PhoneNumber = "+3700000000",
                Gender = 1,
                FkUser = userId
            };

            _context.Teachers.Add(teacher);
            _context.SaveChanges();
        }
        private void RegisterDefaultAdmin()
        {
            int userId = RegisterUser("admin","admin", "admin@email.com", 10);

            Admin admin = new Admin { FkUser = userId };

            _context.Admins.Add(admin);
            _context.SaveChanges();
        }
        public IActionResult ClassInfo()
        {
            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("ClassInfo", "Class");

        }

        private int RegisterUser(string username, string password, string email, int role)
        {
            
            User user = new User
            {
                Username = username,
                Password = Models.User.ComputeSha256Hash(password),
                Email = email,
                Role = role
            };

            _context.Users.Add(user);

            _context.SaveChanges();

            return user.Id;
        }

        private int RegisterGuardian(string name, string surname, string phoneNumber, string email, string address)
        {
            Guardian guardian = new Guardian
            {
                Name = name,
                Surname = surname,
                PhoneNumber = phoneNumber,
                Email = email,
                Address = address
            };

            _context.Guardians.Add(guardian);
            _context.SaveChanges();

            return guardian.Id;
        }

        private Guardian GetGuardianById(int id)
        {
            Guardian guardian = _context.Guardians.FirstOrDefault(g => g.Id == id);

            return guardian;
        }

        private School GetSchoolById(int id)
        {
            School school = _context.Schools.FirstOrDefault(s => s.Id == id);

            return school;
        }

        private string GenerateTemporaryPassword()
        {
            // Define the characters that can be used in the password
            string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";

            // Create a random number generator
            Random random = new Random();

            // Initialize a StringBuilder to store the password
            StringBuilder passwordBuilder = new StringBuilder();

            // Generate 8 random characters and add them to the password
            for (int i = 0; i < 8; i++)
            {
                // Get a random index within the range of allowedChars
                int index = random.Next(0, allowedChars.Length);

                // Append the random character to the password
                passwordBuilder.Append(allowedChars[index]);
            }

            // Convert the StringBuilder to a string and return the temporary password
            string temporaryPassword = passwordBuilder.ToString();

            return temporaryPassword;
        }

    }

}
