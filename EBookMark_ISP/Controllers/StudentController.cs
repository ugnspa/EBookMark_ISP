using EBookMark_ISP.Models;
using EBookMark_ISP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Tls;

namespace EBookMark_ISP.Controllers
{


    public class StudentController : Controller
    {
        private readonly EbookmarkContext _context;

        public StudentController(EbookmarkContext context)
        {

            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Home");
        }

        public bool AccessStudent()
        {
            string username = HttpContext.Session.GetString("Username");
            int? permissions = HttpContext.Session.GetInt32("Permissions");
            if (username == null)
            {
                return false;
            }
            if (permissions != 1)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        public PartialViewResult UpdateRanking(int schedule_id, string selected_subject,string selected_scale, int selected_gender_id) 
        {
            string username = HttpContext.Session.GetString("Username");
            var user_id = _context.Users.FirstOrDefault(a => a.Username == username).Id;
            var student = _context.Students.FirstOrDefault(s => s.FkUser == user_id);
            List<Student> students= new List<Student>();
            switch(selected_scale) // filtering students by scale
            {
                case "Country":

                    students = _context.Students.ToList();
                    break;

                case "City":
                    var city = _context.Schools.FirstOrDefault(a => a.Id == student.FkSchool).City;
                    var schools_in_city = _context.Schools.Where(sch => sch.City == city).Select(sch => sch.Id).ToList();
                    students = _context.Students.Where(st => schools_in_city.Contains(st.FkSchool)).ToList();
                    break;

                case "School":
                    students = _context.Students.Where(st => st.FkSchool == student.FkSchool).ToList();
                    break;
                case "Class":
                    students = _context.Students.Where(st => st.FkClass == student.FkClass).ToList();
                    break;

            }
            //gender filter
            if (selected_gender_id != -1)
            {
                students = students.Where(s => s.Gender == selected_gender_id).ToList();
            }
            if (!students.Contains(student))
            {
                students.Add(student);
            }
           
            //getting valid subject times
            var schedule = _context.Schedules.Find(schedule_id);
            List<StudentAverage> studentAverages= new List<StudentAverage>();

            List<int> valid_subject_times;
            switch (selected_subject)
            {
                case "All":
                    valid_subject_times = _context.SubjectTimes.Where(st => st.StartDate >= schedule.SemesterStart && st.EndDate <= schedule.SemesterEnd)
                        .Select(st => st.Id)
                        .ToList();
                    break;
                default:
                    valid_subject_times = _context.SubjectTimes.Where(st => st.StartDate >= schedule.SemesterStart && st.EndDate <= schedule.SemesterEnd && selected_subject == st.FkSubject)
                        .Select(st=>st.Id)
                        .ToList();
                    break;
            }

            //calculating averages
            foreach(var stud in students)
            {
                var student_schedules = _context.Schedules.Where(sc => sc.FkClass == stud.FkClass).Select(sc=> sc.Id).ToList();
                var student_subject_times = _context.SubjectTimes.Where(st => student_schedules.Contains(st.FkSchedule)).Select(st => st.Id).ToList();
                if(student_subject_times.Any(sst=> valid_subject_times.Contains(sst)))
                {
                    var marks = _context.Marks.Where(mark => mark.FkStudent == stud.FkUser && valid_subject_times.Contains(mark.FkSubjectTime))
                    .Select(mark => mark.Mark1)
                    .ToList();
                    double average = CalculateAverage(marks);
                    studentAverages.Add(new StudentAverage { Student = stud, Average = average });
                }             
            }

            var orderedStudentAverages = studentAverages.OrderByDescending(sa => sa.Average)
                                                 .ToList();
            for (int i = 0; i < orderedStudentAverages.Count; i++)
            {
                orderedStudentAverages[i].rank = i + 1; 
            }
            RankingViewModel viewModel= new RankingViewModel();
           
            viewModel.ThreeBetween = GetThree(orderedStudentAverages, student.FkUser);
            viewModel.total = orderedStudentAverages.Count();
            viewModel.student = student;

            return PartialView("_RankingDetails", viewModel);

        }

        public double CalculateAverage(List<string> marks)
        {
            var numericMarks = marks.Where(mark => int.TryParse(mark, out _))
                                    .Select(int.Parse)
                                    .ToList();

            double sum = numericMarks.Sum();

            double average = numericMarks.Any() ? sum / numericMarks.Count() : 0;

            return average;
        }

        public List<StudentAverage> GetThree(List<StudentAverage> allStudents, int currentStudentId)
        {
            int studentIndex = allStudents.FindIndex(sa => sa.Student.FkUser == currentStudentId);

            if (studentIndex == -1 || allStudents.Count <= 3)
            {
                return allStudents.ToList(); 
            }

            if (studentIndex < 2)
            {
                return allStudents.Take(3).ToList();
            }

            if (studentIndex >= allStudents.Count - 2)
            {
                return allStudents.Skip(Math.Max(0, allStudents.Count - 3)).ToList();
            }

            return allStudents.Skip(studentIndex - 1).Take(3).ToList();
        }


    }
}
