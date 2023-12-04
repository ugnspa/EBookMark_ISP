using Amazon.SimpleEmail.Model;
using EBookMark_ISP.Models;
using EBookMark_ISP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Text;

namespace EBookMark_ISP.Controllers
{
    public class ScheduleController : Controller
    {
        //public IActionResult Index(int selectedWeek = 1)
        //{
        //    Classroom classroom = new Classroom(101, "Lecture Hall", 10, 50, "Science Building");

        //    LessonTime lessonTime = new LessonTime(
        //        start: new DateTime(2023, 11, 7, 9, 0, 0), // November 7, 2023, at 11:00 AM
        //        end: new DateTime(2023, 11, 7, 9, 45, 0),   // Assuming the lesson is 1 hour long, so it ends at 12:00 PM
        //        desc: "Introduction to Programming",
        //        type: "Lecture",
        //        room: classroom,
        //        subject: "Programing"
        //    );
        //    Schedule fullSchedule = new Schedule(
        //    semestarStart: new DateTime(2023, 9, 1),    // Example semester start date
        //    semestarEnd: new DateTime(2023, 12, 31),      // Example semester end date
        //    className: "Computer Science"
        //    );

        //    fullSchedule.Add(lessonTime);






        //    string username = HttpContext.Session.GetString("Username");
        //    if (username != null)
        //    {
        //        int? access = HttpContext.Session.GetInt32("Permissions");
        //        if (access != null)
        //        {
        //            if (access > 1)
        //            {
        //                List<Schedule> list= new List<Schedule>();
        //                list.Add(fullSchedule);
        //                return View("ScheduleList", list);
        //            }
        //        }


        //        var totalWeeks = CalculateTotalWeeks(fullSchedule.SemestarStart, fullSchedule.SemestarEnd);

        //        // Calculate the start and end dates for the selected week
        //        var selectedWeekDates = CalculateWeekDates(fullSchedule.SemestarStart, selectedWeek);

        //        // Filter the lessons for the selected week
        //        var lessonsForSelectedWeek = fullSchedule.Lessons
        //            .Where(lesson => lesson.Start >= selectedWeekDates.Item1 && lesson.End <= selectedWeekDates.Item2)
        //            .ToList();
        //        var weekDates = CalculateWeekDates(fullSchedule.SemestarStart, selectedWeek);
        //        ViewBag.WeekStartDate = weekDates.Item1;
        //        ViewBag.WeekEndDate = weekDates.Item2;
        //        ViewBag.TotalWeeks = totalWeeks;
        //        ViewBag.SelectedWeek = selectedWeek;
        //        ViewBag.WeekDropdown = GenerateWeekDropdown(totalWeeks, selectedWeek);
        //        return View("Schedule", lessonsForSelectedWeek);
        //    }
        //    return View("~/Views/Home/Index.cshtml");
        //}
        private readonly EbookmarkContext _context;

        public ScheduleController(EbookmarkContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("Username");
            if (username != null)
            {
                int? access = HttpContext.Session.GetInt32("Permissions");
                if (access != null && access > 5)
                {
                    //Schedule fullSchedule = CreateFullSchedule(1);
                    //Schedule fullSchedule2 = CreateFullSchedule(2);
                    //List<Schedule> list = new List<Schedule> { fullSchedule, fullSchedule2 };
                    //var schedules = _context.Users.Where(u => u.Admin != null).ToList();
                    var schedules = _context.Schedules.Include(s => s.FkClassNavigation).ToList();
                    return View("ScheduleList", schedules);
                }
                return RedirectToAction("WeeklySchedule");
            }

            return RedirectToAction("Index", "Home");
        }

        //private Schedule CreateFullSchedule(int id)
        //{
        //    //Classroom classroom = new Classroom(1, 101, "Lecture Hall", 10, 50, "Science Building");
        //    LessonTime lessonTime = new LessonTime(
        //        id = 1,
        //        start: new DateTime(2023, 11, 7, 9, 0, 0), // November 7, 2023, at 9:00 AM
        //        end: new DateTime(2023, 11, 7, 9, 45, 0),   // Ends at 9:45 AM
        //        desc: "Introduction to Programming",
        //        type: "Lecture",
        //        room: classroom,
        //        subject: "Programming"
        //    );
        //    Schedule fullSchedule = new Schedule(
        //        id: id,
        //        semestarStart: new DateTime(2023, 9, 1),
        //        semestarEnd: new DateTime(2023, 12, 31),
        //        className: "Computer Science"
        //    );

        //    fullSchedule.Add(lessonTime);

        //    return fullSchedule;
        //}

        //public Schedule GetScheduleById(int id)
        //{
        //    Schedule fullSchedule = CreateFullSchedule(1);
        //    Schedule fullSchedule2 = CreateFullSchedule(2);
        //    List<Schedule> list = new List<Schedule> { fullSchedule, fullSchedule2 };
        //    foreach (Schedule s in list)
        //    {
        //        if (id == s.ScheduleId) return s;
        //    }
        //    return CreateFullSchedule(1);

        //}

        public IActionResult WeeklySchedule(int ?scheduleId, int selectedWeek = 1)
        {

            if (scheduleId == null)
            {
                //retrieve scheduleId by student Id
            }
            Schedule fullSchedule = _context.Schedules.SingleOrDefault(s => s.Id == scheduleId);

            var totalWeeks = CalculateTotalWeeks(fullSchedule.SemesterStart, fullSchedule.SemesterEnd);
            var selectedWeekDates = CalculateWeekDates(fullSchedule.SemesterStart, selectedWeek);


            var lessonsForSelectedWeek = _context.SubjectTimes.Where(l => l.FkSchedule == fullSchedule.Id && 
            l.StartDate >= selectedWeekDates.Item1 && l.EndDate <= selectedWeekDates.Item2).Include(l => l.FkClassroomNavigation).Include(l => l.FkSubjectNavigation).ToList();
            //var lessonsForSelectedWeek = fullSchedule.Lessons
            //    .Where(lesson => lesson.Start >= selectedWeekDates.Item1 && lesson.End <= selectedWeekDates.Item2)
            //    .ToList();

            ViewBag.WeekStartDate = selectedWeekDates.Item1;
            ViewBag.WeekEndDate = selectedWeekDates.Item2;
            ViewBag.TotalWeeks = totalWeeks;
            ViewBag.SelectedWeek = selectedWeek;
            ViewBag.ScheduleId = fullSchedule.Id;
            ViewBag.WeekDropdown = GenerateWeekDropdown(totalWeeks, selectedWeek, fullSchedule.SemesterStart);

            // Assuming you have a view that takes a single Schedule object and displays it weekly
            return View("Schedule", lessonsForSelectedWeek);
        }

        public IActionResult CreateSchedule()
        {
            var classes = _context.Classes.ToList();
            return View("CreateSchedule", classes);
        }

        public IActionResult AddSchedule(int ClassId, DateTime SemestarStart, DateTime SemestarEnd)
        {
            var schedule = new Schedule
            {
                SemesterStart = SemestarStart,
                SemesterEnd = SemestarEnd,
                FkClass = ClassId.ToString()
            };
            if (!ValidateSchedule(schedule))
            {
                ViewBag.Errors = "Nurodytame semestro laikotarpyje jau egzistuoja tvarkaraštis šiai klasei";
                var classes = _context.Classes.ToList();
                return View("CreateSchedule", classes);
            }
            _context.Schedules.Add(schedule);
            _context.SaveChanges();

            return RedirectToAction("EditSchedule", new { scheduleId = schedule.Id });
        }

        public IActionResult GenerateAndAddSchedule()
        {
            return RedirectToAction("EditSchedule", new { scheduleId = 1 });
        }

        public IActionResult GenerateSchedule()
        {

            var subjects = _context.Subjects.ToList();
            var classes = _context.Classes.ToList();
            ViewBag.classes = classes; 
            return View(subjects);
        }

        public IActionResult EditSchedule(int scheduleId)
        {
            Schedule fullSchedule = _context.Schedules
                .Include(s => s.FkClassNavigation)
                .Include(s => s.SubjectTimes)
                .ThenInclude(st => st.FkSubjectNavigation)
                .SingleOrDefault(s => s.Id == scheduleId);
            //Check for conflicts
            var classes = _context.Classes.ToList();
            ViewBag.Classes = classes;
            return View(fullSchedule);
        }

        //[HttpPost]
        public IActionResult UpdateSchedule(Schedule model, DateTime SemestarStart, DateTime SemestarEnd, string ClassId)
        {
            model.SemesterStart = SemestarStart;
            model.SemesterEnd = SemestarEnd;
            model.FkClass = ClassId;
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult EditLessonTime(int scheduleId, int LessonTimeid)
        {
            var subjectTime = _context.SubjectTimes.Find(LessonTimeid);
            var Classroom = _context.Classrooms.ToList();
            var subjects = _context.Subjects.ToList();
            var types = _context.SubjectTypes.ToList();
            ViewBag.Classrooms = Classroom;
            ViewBag.Subjects = subjects;
            ViewBag.Types = types;
            ViewBag.Schdeule = scheduleId;

            return View(subjectTime);
        }

        public IActionResult DeleteLessonTime(int LessonTimeid, int scheduleId)
        {
            var subjectTime = _context.SubjectTimes.Find(LessonTimeid);
            if (subjectTime != null)
            {
                _context.SubjectTimes.Remove(subjectTime);
            }
            _context.SaveChanges();
            return RedirectToAction("EditSchedule", new { scheduleId = scheduleId });
        }

        public IActionResult UpdateLessonTime(SubjectTime model, DateTime Start, DateTime End, string Description, int SubjectTypeId,
            string SubjectId, int classroomId, int scheduleId, string AddWeekly)
        {
            bool addWeeklyBool = AddWeekly?.ToLower() == "true";

            model.StartDate = Start;
            model.EndDate = End;
            model.Descrtiption = Description;
            model.FkSubject = SubjectId;
            model.Type = SubjectTypeId;
            if (classroomId == 0) 
            {
                model.FkClassroom = null;
            }
            else
            {
                model.FkClassroom = classroomId;
            }
            
            model.FkSchedule= scheduleId;

            if (addWeeklyBool)
            {
                //var oldSubjectTime = _context.SubjectTimes.Find(model.Id);
                //if (Start != )
                UpdateForAllSemestarWeeks(model);
            }
            else
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
            }
            
            return RedirectToAction("EditSchedule", new { scheduleId = scheduleId });
        }

        public IActionResult AddLessonTime(int scheduleId)
        {
            var Classroom = _context.Classrooms.ToList();
            var subjects = _context.Subjects.ToList();
            var types = _context.SubjectTypes.ToList();
            ViewBag.Classrooms = Classroom;
            ViewBag.Subjects = subjects;
            ViewBag.Types = types;

            //ViewBag.ClassroomSelectList = new SelectList(classroomSelectItems, "number", "displayValue");

            // Pass a new instance of LessonTime to ensure fields are empty


            return View(scheduleId);
        }

        public IActionResult CreateLessonTime(DateTime Start, DateTime End, string Description, int SubjectTypeId, string SubjectId, int classroomId,
    int scheduleId, string AddWeekly)
        {
            bool addWeeklyBool = AddWeekly?.ToLower() == "true";

            SubjectTime subjectTime = new SubjectTime
            {
                StartDate = Start,
                EndDate = End,
                Descrtiption = Description,
                Type = SubjectTypeId,
                FkSubject = SubjectId,
                FkClassroom = classroomId,
                FkSchedule = scheduleId
            };
            if (classroomId == 0)
            {
                subjectTime.FkClassroom = null;
            }
            if (addWeeklyBool)
            {
                AddForAllSemestarWeeks(subjectTime);
            }
            else
            {
                _context.SubjectTimes.Add(subjectTime);
                _context.SaveChanges();
            }
            return RedirectToAction("EditSchedule", new { scheduleId = scheduleId });
        }

        private int CalculateTotalWeeks(DateTime startDate, DateTime endDate)
        {
            // Calculate the number of weeks between two dates
            var totalDays = (endDate - startDate).Days;
            var totalWeeks = totalDays / 7;
            return totalWeeks + 1;
        }

        private Tuple<DateTime, DateTime> CalculateWeekDates(DateTime semesterStart, int weekNumber)
        {
            // Calculate the start and end dates for a specific week
            var startDate = semesterStart.AddDays((weekNumber - 1) * 7);
            var endDate = startDate.AddDays(6);
            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        private string GenerateWeekDropdown(int totalWeeks, int selectedWeek, DateTime semesterStart)
        {
            var dropdown = new StringBuilder();
            for (var i = 1; i <= totalWeeks; i++)
            {
                dropdown.AppendLine($"<option value=\"{i}\"{(i == selectedWeek ? " selected" : "")}>Week {i} {CalculateWeekDates(semesterStart, i).Item1.ToString("yyyy-MM-dd")}</option>");
            }
            return dropdown.ToString();
        }

        public IActionResult DeleteSchedule(int scheduleId)
        {
            var schedule = _context.Schedules.Find(scheduleId);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public void AddForAllSemestarWeeks(SubjectTime originalSubjectTime)
        {
            var schedule = _context.Schedules.Find(originalSubjectTime.FkSchedule);
            DateTime startDate = originalSubjectTime.StartDate;
            DateTime endDate = originalSubjectTime.EndDate;

            while (startDate <= schedule.SemesterEnd)
            {
                if (startDate >= schedule.SemesterStart)
                {
                    SubjectTime newSubjectTime = new SubjectTime
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Descrtiption = originalSubjectTime.Descrtiption,
                        Type = originalSubjectTime.Type,
                        FkSubject = originalSubjectTime.FkSubject,
                        FkClassroom = originalSubjectTime.FkClassroom,
                        FkSchedule = originalSubjectTime.FkSchedule
                    };

                    // Validate and add newSubjectTime
                    _context.SubjectTimes.Add(newSubjectTime);
                }

                // Increment dates by 7 days
                startDate = startDate.AddDays(7);
                endDate = endDate.AddDays(7);
            }

            _context.SaveChanges();
        }

        public void UpdateForAllSemestarWeeks(SubjectTime originalSubjectTime)
        {
            var schedule = _context.Schedules.Find(originalSubjectTime.FkSchedule);
            DateTime startDate = originalSubjectTime.StartDate;
            DateTime endDate = originalSubjectTime.EndDate;

            while (startDate <= schedule.SemesterEnd)
            {
                if (startDate >= schedule.SemesterStart)
                {
                    var newSubTime = _context.SubjectTimes.SingleOrDefault(s => s.StartDate == startDate && s.EndDate == endDate && s.FkSchedule == schedule.Id);
                    if (newSubTime != null)
                    {
                        newSubTime.StartDate = startDate;
                        newSubTime.EndDate = endDate;
                        newSubTime.Descrtiption = originalSubjectTime.Descrtiption;
                        newSubTime.Type = originalSubjectTime.Type;
                        newSubTime.FkSubject = originalSubjectTime.FkSubject;
                        newSubTime.FkClassroom = originalSubjectTime.FkClassroom;
                        newSubTime.FkSchedule = originalSubjectTime.FkSchedule;

                        // Validate and add newSubjectTime
                        _context.Entry(newSubTime).State = EntityState.Modified;
                    }
                }

                // Increment dates by 7 days
                startDate = startDate.AddDays(7);
                endDate = endDate.AddDays(7);
            }

            _context.SaveChanges();
        }

        public bool ValidateSchedule(Schedule schedule)
        {
            var schedules = _context.Schedules.Where(s => s.FkClass == schedule.FkClass &&
            (s.SemesterEnd > schedule.SemesterStart && schedule.SemesterEnd > s.SemesterStart)).ToList();
            if (schedules.Count == 0)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }


    }
}
