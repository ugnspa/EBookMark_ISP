//using EBookMark_ISP.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Text;

//namespace EBookMark_ISP.Controllers
//{
//    public class ScheduleController : Controller
//    {
//        //public IActionResult Index(int selectedWeek = 1)
//        //{
//        //    Classroom classroom = new Classroom(101, "Lecture Hall", 10, 50, "Science Building");

//        //    LessonTime lessonTime = new LessonTime(
//        //        start: new DateTime(2023, 11, 7, 9, 0, 0), // November 7, 2023, at 11:00 AM
//        //        end: new DateTime(2023, 11, 7, 9, 45, 0),   // Assuming the lesson is 1 hour long, so it ends at 12:00 PM
//        //        desc: "Introduction to Programming",
//        //        type: "Lecture",
//        //        room: classroom,
//        //        subject: "Programing"
//        //    );
//        //    Schedule fullSchedule = new Schedule(
//        //    semestarStart: new DateTime(2023, 9, 1),    // Example semester start date
//        //    semestarEnd: new DateTime(2023, 12, 31),      // Example semester end date
//        //    className: "Computer Science"
//        //    );

//        //    fullSchedule.Add(lessonTime);



            

            
//        //    string username = HttpContext.Session.GetString("Username");
//        //    if (username != null)
//        //    {
//        //        int? access = HttpContext.Session.GetInt32("Permissions");
//        //        if (access != null)
//        //        {
//        //            if (access > 1)
//        //            {
//        //                List<Schedule> list= new List<Schedule>();
//        //                list.Add(fullSchedule);
//        //                return View("ScheduleList", list);
//        //            }
//        //        }


//        //        var totalWeeks = CalculateTotalWeeks(fullSchedule.SemestarStart, fullSchedule.SemestarEnd);

//        //        // Calculate the start and end dates for the selected week
//        //        var selectedWeekDates = CalculateWeekDates(fullSchedule.SemestarStart, selectedWeek);

//        //        // Filter the lessons for the selected week
//        //        var lessonsForSelectedWeek = fullSchedule.Lessons
//        //            .Where(lesson => lesson.Start >= selectedWeekDates.Item1 && lesson.End <= selectedWeekDates.Item2)
//        //            .ToList();
//        //        var weekDates = CalculateWeekDates(fullSchedule.SemestarStart, selectedWeek);
//        //        ViewBag.WeekStartDate = weekDates.Item1;
//        //        ViewBag.WeekEndDate = weekDates.Item2;
//        //        ViewBag.TotalWeeks = totalWeeks;
//        //        ViewBag.SelectedWeek = selectedWeek;
//        //        ViewBag.WeekDropdown = GenerateWeekDropdown(totalWeeks, selectedWeek);
//        //        return View("Schedule", lessonsForSelectedWeek);
//        //    }
//        //    return View("~/Views/Home/Index.cshtml");
//        //}

//        public IActionResult Index()
//        {
//            string username = HttpContext.Session.GetString("Username");
//            if (username != null)
//            {
//                int? access = HttpContext.Session.GetInt32("Permissions");
//                if (access != null && access > 5)
//                {
//                    Schedule fullSchedule = CreateFullSchedule(1);
//                    Schedule fullSchedule2 = CreateFullSchedule(2);
//                    List<Schedule> list = new List<Schedule> { fullSchedule, fullSchedule2 };
//                    return View("ScheduleList", list);
//                }
//                return RedirectToAction("WeeklySchedule");
//            }

//            return RedirectToAction("Index", "Home");
//        }

//        private Schedule CreateFullSchedule(int id)
//        {
//            Classroom classroom = new Classroom(1, 101, "Lecture Hall", 10, 50, "Science Building");
//            LessonTime lessonTime = new LessonTime(
//                id = 1,
//                start: new DateTime(2023, 11, 7, 9, 0, 0), // November 7, 2023, at 9:00 AM
//                end: new DateTime(2023, 11, 7, 9, 45, 0),   // Ends at 9:45 AM
//                desc: "Introduction to Programming",
//                type: "Lecture",
//                room: classroom,
//                subject: "Programming"
//            );
//            Schedule fullSchedule = new Schedule(
//                id: id,
//                semestarStart: new DateTime(2023, 9, 1),
//                semestarEnd: new DateTime(2023, 12, 31),
//                className: "Computer Science"
//            );

//            fullSchedule.Add(lessonTime);

//            return fullSchedule;
//        }

//        public Schedule GetScheduleById(int id)
//        {
//            Schedule fullSchedule = CreateFullSchedule(1);
//            Schedule fullSchedule2 = CreateFullSchedule(2);
//            List<Schedule> list = new List<Schedule> { fullSchedule, fullSchedule2 };
//            foreach (Schedule s in list)
//            {
//                if (id == s.ScheduleId) return s;
//            }
//            return CreateFullSchedule(1);

//        }

//        public IActionResult WeeklySchedule(int scheduleId = 1, int selectedWeek = 1)
//        {
//            Schedule fullSchedule = GetScheduleById(scheduleId);

//            var totalWeeks = CalculateTotalWeeks(fullSchedule.SemestarStart, fullSchedule.SemestarEnd);
//            var selectedWeekDates = CalculateWeekDates(fullSchedule.SemestarStart, selectedWeek);

//            var lessonsForSelectedWeek = fullSchedule.Lessons
//                .Where(lesson => lesson.Start >= selectedWeekDates.Item1 && lesson.End <= selectedWeekDates.Item2)
//                .ToList();

//            ViewBag.WeekStartDate = selectedWeekDates.Item1;
//            ViewBag.WeekEndDate = selectedWeekDates.Item2;
//            ViewBag.TotalWeeks = totalWeeks;
//            ViewBag.SelectedWeek = selectedWeek;
//            ViewBag.WeekDropdown = GenerateWeekDropdown(totalWeeks, selectedWeek);

//            // Assuming you have a view that takes a single Schedule object and displays it weekly
//            return View("Schedule", lessonsForSelectedWeek);
//        }

//        public IActionResult CreateSchedule()
//        {
//            return View("CreateSchedule");
//        }

//        public IActionResult AddSchedule()
//        {
//            return RedirectToAction("EditSchedule", new { scheduleId = 1 });
//        }

//        public IActionResult GenerateAndAddSchedule()
//        {
//            return RedirectToAction("EditSchedule", new { scheduleId = 1 });
//        }

//        public IActionResult GenerateSchedule()
//        {
//            List<Subject> list = new List<Subject>
//            {
//                new Subject("CS101", "Introduction to Computer Science", "An introductory course on computer science.", "English"),
//                new Subject("MATH201", "Calculus I", "A basic course on calculus.", "English"),
//                new Subject("PHYS101", "Physics Fundamentals", "An introductory course on physics.", "English")
//            };

//            return View(list);
//        }

//        public IActionResult EditSchedule(int scheduleId = 1)
//        {
//            Schedule fullSchedule = GetScheduleById(scheduleId);
//            return View(fullSchedule);
//        }

//        [HttpPost]
//        public IActionResult UpdateSchedule(Schedule model)
//        {
//            return RedirectToAction("index");
//        }

//        public IActionResult EditLessonTime(int id)
//        {
//            Classroom classroom = new Classroom(1, 101, "Lecture Hall", 10, 50, "Science Building");
//            Classroom classroom2 = new Classroom(2, 102, "Lecture Hall", 10, 50, "Science Building");
//            LessonTime lessonTime = new LessonTime(
//                id = 1,
//                start: new DateTime(2023, 11, 7, 9, 0, 0), // November 7, 2023, at 9:00 AM
//                end: new DateTime(2023, 11, 7, 9, 45, 0),   // Ends at 9:45 AM
//                desc: "Introduction to Programming",
//                type: "Lecture",
//                room: classroom,
//                subject: "Programming"
//            );
//            List<Classroom> classrooms = new List<Classroom>();
//            classrooms.Add(classroom);
//            classrooms.Add(classroom2);
//            ViewBag.ClassroomSelectList = new SelectList(classrooms, "number", "FullInfo", lessonTime.room?.number);
//            return View(lessonTime);
//        }

//        public IActionResult DeleteLessonTime(int id)
//        {
//            return RedirectToAction("EditSchedule");
//        }

//        public IActionResult UpdateLessonTime()
//        {
//            return RedirectToAction("EditSchedule");
//        }

//        public IActionResult AddLessonTime()
//        {
//            Classroom classroom = new Classroom(1, 101, "Lecture Hall", 10, 50, "Science Building");
//            Classroom classroom2 = new Classroom(2, 102, "Lab", 10, 30, "Engineering Building");
//            List<Classroom> classrooms = new List<Classroom>() { classroom, classroom2 };

//            // Prepare the select list with a formatted display string
//            var classroomSelectItems = classrooms.Select(c => new {
//                number = c.number,
//                displayValue = $"Room {c.number} - {c.usage} - {c.building}"
//            }).ToList();

//            ViewBag.ClassroomSelectList = new SelectList(classroomSelectItems, "number", "displayValue");

//            // Pass a new instance of LessonTime to ensure fields are empty
//            var newLessonTime = new LessonTime();

//            return View(newLessonTime);
//        }

//        public IActionResult CreateLessonTime()
//        {
//            return RedirectToAction("EditSchedule");
//        }

//        private int CalculateTotalWeeks(DateTime startDate, DateTime endDate)
//        {
//            // Calculate the number of weeks between two dates
//            var totalDays = (endDate - startDate).Days;
//            var totalWeeks = totalDays / 7;
//            return totalWeeks + 1;
//        }

//        private Tuple<DateTime, DateTime> CalculateWeekDates(DateTime semesterStart, int weekNumber)
//        {
//            // Calculate the start and end dates for a specific week
//            var startDate = semesterStart.AddDays((weekNumber - 1) * 7);
//            var endDate = startDate.AddDays(6);
//            return new Tuple<DateTime, DateTime>(startDate, endDate);
//        }

//        private string GenerateWeekDropdown(int totalWeeks, int selectedWeek)
//        {
//            var dropdown = new StringBuilder();
//            for (var i = 1; i <= totalWeeks; i++)
//            {
//                dropdown.AppendLine($"<option value=\"{i}\"{(i == selectedWeek ? " selected" : "")}>Week {i}</option>");
//            }
//            return dropdown.ToString();
//        }

//        public IActionResult DeleteSchedule(int scheduleId)
//        {
//            return RedirectToAction("index");
//        }


//    }
//}
