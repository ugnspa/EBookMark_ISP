using EBookMark_ISP.Models;
using EBookMark_ISP.Services;
using EBookMark_ISP.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Tls;
using System.Text;
using System.Xml.Linq;
using ZstdSharp.Unsafe;
using static Azure.Core.HttpHeader;

namespace EBookMark_ISP.Controllers
{
    public class UserController : Controller
    {

        private readonly EbookmarkContext _context;
        private readonly IEmailService _emailService;

        public UserController(EbookmarkContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
        public IActionResult GradeBook(int? student_id)
        {
            if (!AccessWatcher())
            {
                return RedirectToAction("Dashboard", "Home");
            }
            string username = HttpContext.Session.GetString("Username");
            var user_id = student_id!=null?student_id: _context.Users.FirstOrDefault(us => us.Username == username).Id;
            Student student = _context.Students.FirstOrDefault(st => st.FkUser == user_id);

            var viewModel = new EBookMark_ISP.ViewModels.GradeBookViewModel
            {
                student = student,
                schedules = new Dictionary<Schedule, List<GradeBookViewModel.SubjectMarks>>()
            };
            if (student_id !=null)
            {
                var ud = _context.Users.FirstOrDefault(us => us.Username == username).Id;
                var teacher_subjects = _context.Subjects.Where(sb => sb.FkTeacher== ud).Select(sb => sb.Code).ToList();
                viewModel.teachers_subject = teacher_subjects;
            }

            var student_schedules = _context.Schedules.Where(sh => sh.FkClass == student.FkClass).ToList();

            foreach (var schedule in student_schedules)
            {
                List<GradeBookViewModel.SubjectMarks> list = new List<GradeBookViewModel.SubjectMarks>();

                var student_subjects_curr = _context.SubjectTimes.Where(st => st.FkSchedule == schedule.Id).Select(su => su.FkSubject).Distinct().ToList();
                foreach(var subject in student_subjects_curr)
                {
                    GradeBookViewModel.SubjectMarks subject_marks= new GradeBookViewModel.SubjectMarks();
                    subject_marks.subject = subject;
                    var all_marks = _context.Marks.Where(ma => ma.FkStudent == student.FkUser).ToList();
                    List<GradeBookViewModel.MarkTime> marks = new List<GradeBookViewModel.MarkTime>();
                    foreach(var mark in all_marks)
                    {
                        var subjectTime = _context.SubjectTimes.Where(st => st.Id == mark.FkSubjectTime && schedule.Id == st.FkSchedule && subject == st.FkSubject).FirstOrDefault();

                        if (subjectTime != null)
                        {
                            GradeBookViewModel.MarkTime mark_time = new GradeBookViewModel.MarkTime
                            {
                                time = subjectTime.StartDate,
                                mark = mark
                            };
                            marks.Add(mark_time);
                        }
                    }
                    subject_marks.marksTimes = marks;
                    list.Add(subject_marks);
                }
                viewModel.schedules[schedule] = list;
            }

            int? permissions = HttpContext.Session.GetInt32("Permissions");
            ViewBag.Permissions = permissions;
            string? message = HttpContext.Session.GetString("Message");
            if (message != null)
            {
                ViewBag.Message = message;
                HttpContext.Session.Remove("Message");
            }


            return View(viewModel);
        }

        public IActionResult ChangePassword()
        {
            string username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            string password = _context.Users.Where(u => u.Username == username).First().Password;

            return View((object)password);
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            string username = HttpContext.Session.GetString("Username");
            if (username == null) {
                return RedirectToAction("Dashboard", "Home");
            }
             
            try
            {
                User user = _context.Users.FirstOrDefault(u => u.Username == username);
                user.Password = Hash.ComputeSha256Hash(newPassword);
                _context.SaveChanges();
                HttpContext.Session.SetString("Message", "Password updated successfully");
            }
            catch
            {
                HttpContext.Session.SetString("Message", "Password was unable to update");
                return RedirectToAction("Dashboard", "Home");
            }

            return RedirectToAction("Dashboard","Home");
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
            var admins = _context.Users.Where(u => u.Admin != null).ToList();
            var students = _context.Students.Include(s => s.FkUserNavigation).ToList();
            var teachers = _context.Teachers.Include(t => t.FkUserNavigation).ToList();

            // Pass the lists to the view
            ViewBag.Admins = admins;
            ViewBag.Students = students;
            ViewBag.Teachers = teachers;
            return View();
        }
        public IActionResult Register()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            string adminError = HttpContext.Session.GetString("AdminError");
            if(adminError != null)
            {
                HttpContext.Session.Remove("AdminError");
                ViewBag.ErrorMessage = adminError;
            }

            var genders = _context.Genders.ToList();
            ViewBag.GenderOptions = new SelectList(genders, "Id", "Name");

            var schools = _context.Schools.ToList();
            ViewBag.SchoolOptions = new SelectList(schools, "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult Register(string email,
            string name, string surname, DateTime birthDate,
            string gender, string personalID, bool hasStudentID,
            string guardianName, string guardianSurname,
            string guardianPhoneNumber, string guardianEmail, string guardianAdress, int school)
        {

            string tempPassword = GenerateTemporaryPassword();
            string username = GenerateUsername(name, surname);
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

            SendPasswordEmail(username, tempPassword, email);

            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Userlist");
        }

        [HttpPost]
        public IActionResult RegisterTeacher(string email,
            string name, string surname, DateTime employmentDate,
            int gender, string phoneNumber, string personalID, int school)
        {

            string tempPassword = GenerateTemporaryPassword();
            string username = GenerateUsername(name, surname);

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
            SendPasswordEmail(username, tempPassword, email);



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

            List<User> users = _context.Users.Where(u => u.Username == username).ToList();

            if(users.Count > 0)
            {
                Console.WriteLine("USERNAME TAKEN");
                HttpContext.Session.SetString("AdminError", "This username is already taken");
                return RedirectToAction("Register");
            }

            string tempPassword = GenerateTemporaryPassword();

            int userId = RegisterUser(username, tempPassword, email, 10);

            Admin admin = new Admin { FkUser = userId };
            _context.Admins.Add(admin);
            _context.SaveChanges();
            SendPasswordEmail(username, tempPassword, email);
            string currUsername = HttpContext.Session.GetString("Username");
            if (currUsername == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return RedirectToAction("Userlist");
        }

        [HttpGet]
        public IActionResult Remove(int id, string type)
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null || permissions == null || permissions < 9)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            Console.WriteLine("Remove user:");
            Console.WriteLine($"id: {id}");
            Console.WriteLine($"type: {type}");

            //List<Student> classStudents = _context.Students.Where(s => s.FkClass == code).ToList();

            switch (type)
            {
                case "admin":
                    Admin adminToRemove = _context.Admins.FirstOrDefault(a => a.FkUser == id);
                    if (adminToRemove != null)
                        _context.Admins.Remove(adminToRemove);
                    break;
                case "student":
                    Student studentToRemove = _context.Students.FirstOrDefault(a => a.FkUser == id);
                    if (studentToRemove != null)
                    {
                        if(studentToRemove.FkClass != null)
                        {
                            _context.Classes.FirstOrDefault(c => c.Code == studentToRemove.FkClass).StudentsCount--;
                        }
                        _context.Students.Remove(studentToRemove);
                    }
                        
                    break;
                case "teacher":
                    Teacher teacherToRemove = _context.Teachers.FirstOrDefault(a => a.FkUser == id);
                    if (teacherToRemove != null)
                        _context.Teachers.Remove(teacherToRemove);
                    break;
                default:
                    return RedirectToAction("Userlist");
            }


            User userToRemove = _context.Users.FirstOrDefault(u => u.Id == id);
            if (userToRemove != null)
                _context.Users.Remove(userToRemove);

            _context.SaveChanges();
            return RedirectToAction("Userlist");
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

        private void SendPasswordEmail(string username, string password, string email)
        {
            string content = string.Format("Your usename: {0}\nYour password: {1}\n\nYou can change the password after logging in.", username, password);
            _emailService.SendEmailAsync("vjaras202@gmail.com", "Login credentials", content);
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
            int userId = RegisterUser("admin", "admin", "admin@email.com", 10);

            Admin admin = new Admin { FkUser = userId };

            _context.Admins.Add(admin);
            _context.SaveChanges();
        }

        private void RegisterDefaultStudent(string name, string surname, int schoolId)
        {
            string username = (name.Substring(0, 3) + surname.Substring(0, 3)).ToLower();
            int userId = RegisterUser(username, username, username + "@email.com", 1);

            var student = new Student
            {
                PersonalCode = GenerateRandomNumberString(11),
                Name = name,
                Surname = surname,
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

        private void RegisterDefaultTeacher(string name, string surname)
        {
            string username = (name.Substring(0, 3) + surname.Substring(0, 3)).ToLower();
            int userId = RegisterUser(username, username, username + "@email.com", 5);

            Teacher teacher = new Teacher
            {
                PersonalCode = GenerateRandomNumberString(11),
                Name = name,
                Surname = surname,
                EmploymentDate = DateTime.Now,
                PhoneNumber = "+370" + GenerateRandomNumberString(8),
                Gender = 1,
                FkUser = userId
            };

            _context.Teachers.Add(teacher);
            _context.SaveChanges();
        }

        private int RegisterUser(string username, string password, string email, int role)
        {

            User user = new User
            {
                Username = username,
                Password = Hash.ComputeSha256Hash(password),
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

        private string GenerateUsername(string name, string surname)
        {
            string username = (name.Substring(0, 3) + surname.Substring(0, 3)).ToLower();

            List<User> users = _context.Users.Where(u => u.Username.StartsWith(username)).OrderBy(u => username).ToList();

            if (users.Count == 0)
                return username;

            foreach(User u in users)
            {
                Console.WriteLine($"Username: {u.Username} number {u.Username.Remove(0, username.Length)}");
            }

            User lastUser = users.Last();

            int lastUserNumber;

            Int32.TryParse(lastUser.Username.Remove(0, username.Length), out lastUserNumber);

            lastUserNumber++;
            username = username + lastUserNumber.ToString();

            return username;
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

        public static string GenerateRandomNumberString(int length)
        {
            Random random = new Random();
            StringBuilder randomNumberString = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                // Generate a single digit (0-9) and append it to the string
                randomNumberString.Append(random.Next(0, 10));
            }

            return randomNumberString.ToString();
        }
    }
}
