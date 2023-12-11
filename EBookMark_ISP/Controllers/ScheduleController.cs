using Amazon.SimpleEmail.Model;
using EBookMark_ISP.Models;
using EBookMark_ISP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace EBookMark_ISP.Controllers
{
    public class ScheduleController : Controller
    {
        
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
                if (access != null && access >= 5)
                {
                    //Schedule fullSchedule = CreateFullSchedule(1);
                    //Schedule fullSchedule2 = CreateFullSchedule(2);
                    //List<Schedule> list = new List<Schedule> { fullSchedule, fullSchedule2 };
                    //var schedules = _context.Users.Where(u => u.Admin != null).ToList();
                    if (access == 5)
                    {
                        var teacher = _context.Users.FirstOrDefault(t => t.Username == username);
                        var teacher_schedules = _context.Subjects
                                          .Where(s => s.FkTeacher == teacher.Id)
                                          .SelectMany(s => s.SubjectTimes)
                                          .Select(st => st.FkSchedule)
                                          .Distinct()
                                          .ToList();
                        var schedules = _context.Schedules
                            .Where(s => teacher_schedules.Contains(s.Id))
                            .Include(s => s.FkClassNavigation)
                            .OrderBy(s => s.SemesterStart)
                            .ThenBy(s => s.FkClassNavigation.Name)
                            .ToList();

                        ViewBag.Permissions = access;
                        return View("ScheduleList", schedules);
                    }
                    else
                    {
                        var schedules = _context.Schedules.Include(s => s.FkClassNavigation).OrderBy(s => s.SemesterStart).ThenBy(s => s.FkClassNavigation.Name).
                        ToList();
                        ViewBag.Permissions = access;
                        return View("ScheduleList", schedules);
                    }
                }
                return RedirectToAction("WeeklySchedule");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult WeeklySchedule(int ?scheduleId, int selectedWeek = 1)
        {

            if (scheduleId == null || HttpContext.Session.GetInt32("Permissions") == 1)
            {
                string code = _context.Students.Include(s => s.FkUserNavigation).
                    FirstOrDefault(s => s.FkUserNavigation.Username == HttpContext.Session.GetString("Username")).FkClass;
                scheduleId = _context.Schedules.Where(s => s.FkClass == code && s.SemesterStart <= DateTime.Now && s.SemesterEnd >= DateTime.Now).FirstOrDefault().Id;

                if (scheduleId == null)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            Schedule fullSchedule = _context.Schedules.SingleOrDefault(s => s.Id == scheduleId);

            var totalWeeks = CalculateTotalWeeks(fullSchedule.SemesterStart, fullSchedule.SemesterEnd);
            var selectedWeekDates = CalculateWeekDates(fullSchedule.SemesterStart, selectedWeek);


            var lessonsForSelectedWeek = _context.SubjectTimes.Where(l => l.FkSchedule == fullSchedule.Id && 
            l.StartDate >= selectedWeekDates.Item1 && l.EndDate <= selectedWeekDates.Item2.AddDays(1)).Include(l => l.FkClassroomNavigation).Include(l => l.FkSubjectNavigation).Include(l => l.TypeNavigation).ToList();
            //var lessonsForSelectedWeek = fullSchedule.Lessons
            //    .Where(lesson => lesson.Start >= selectedWeekDates.Item1 && lesson.End <= selectedWeekDates.Item2)
            //    .ToList();

            ViewBag.WeekStartDate = selectedWeekDates.Item1;
            ViewBag.WeekEndDate = selectedWeekDates.Item2;
            ViewBag.TotalWeeks = totalWeeks;
            ViewBag.SelectedWeek = selectedWeek;
            ViewBag.ScheduleId = fullSchedule.Id;
            ViewBag.WeekDropdown = GenerateWeekDropdown(totalWeeks, selectedWeek, fullSchedule.SemesterStart);
            ViewBag.Access = HttpContext.Session.GetInt32("Permissions");

            // Assuming you have a view that takes a single Schedule object and displays it weekly
            return View("Schedule", lessonsForSelectedWeek);
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

        public IActionResult CreateSchedule()
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var classes = _context.Classes.ToList();
            return View("CreateSchedule", classes);
        }

        public IActionResult AddSchedule(int ClassId, DateTime SemestarStart, DateTime SemestarEnd)
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var schedule = new Schedule
            {
                SemesterStart = SemestarStart,
                SemesterEnd = SemestarEnd,
                FkClass = ClassId.ToString()
            };
            if (!ValidateSchedule(schedule)) // Assuming ValidateSchedule is your custom validation method
            {
                // Add a model error
                ModelState.AddModelError("", "Nurodytame semestro laikotarpyje jau egzistuoja tvarkaraštis šiai klasei");

                // Prepare any additional data needed for the view
                var classes = _context.Classes.ToList();

                // Return the same view with the model and errors
                return View("CreateSchedule", classes);
            }
            _context.Schedules.Add(schedule);
            _context.SaveChanges();

            return RedirectToAction("EditSchedule", new { scheduleId = schedule.Id });
        }

        [HttpPost]
        public IActionResult GenerateAndAddSchedule(DateTime semesterStart, DateTime semesterEnd, string className, List<string> Subjects, List<int> Amounts, List<string> ClassroomTypes)
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {

                return RedirectToAction("Dashboard", "Home");
            }
            //validation
            //lesson length = 45min

            if (Amounts.Sum() > 30)
            {
                ModelState.AddModelError("", "Savaitei nurodyta per daug pamoku");
                var subjects = _context.Subjects.ToList();
                var classes = _context.Classes.ToList();
                var classrooms = _context.Classrooms.ToList();
                ViewBag.classes = classes;
                ViewBag.classrooms = classrooms;
                return View("GenerateSchedule", subjects);
            }
            var schedule = new Schedule
            {
                SemesterStart = semesterStart,
                SemesterEnd = semesterEnd,
                //className has code value
                FkClass = className.ToString()
            };
           
            if (!ValidateSchedule(schedule))
            {
                ModelState.AddModelError("", "Nurodytame semestro laikotarpyje jau egzistuoja tvarkaraštis šiai klasei");
                var subjects = _context.Subjects.ToList();
                var classes = _context.Classes.ToList();
                var classrooms = _context.Classrooms.ToList();
                ViewBag.classes = classes;
                ViewBag.classrooms = classrooms;
                return View("GenerateSchedule", subjects);
            }
            _context.Schedules.Add(schedule);
            _context.SaveChanges();


            var totalWeeks = CalculateTotalWeeks(schedule.SemesterStart, schedule.SemesterEnd);
            for (var i = 1; i <= totalWeeks; i++)
            {
                Tuple<DateTime, DateTime> selectedWeekDates = CalculateWeekDates(schedule.SemesterStart, i);
                generateWeeklySchedule(schedule, Subjects, new List<int>(Amounts), ClassroomTypes, selectedWeekDates);
            }




            //for (DateTime i = semesterStart; i < semesterStart.AddDays(7); i = i.AddDays(1))
            //{
            //    for (DateTime j = i.AddHours(8); j < i.AddHours)
            //}
            //for (int i = 0; i < Subjects.Count; i++)
            //{
            //    for (int j = 0; j < Amounts[i]; j++)
            //    {
            //        //for (DateTime t = )
            //    }
            //}
            return RedirectToAction("EditSchedule", new { scheduleId = schedule.Id });
        }

        public void generateWeeklySchedule(Schedule schedule, List<string> Subjects, List<int> Amounts, List<string> ClassroomTypes, Tuple<DateTime, DateTime> selectedWeekDates)
        {
            
            int classSize = _context.Classes.Find(schedule.FkClass).StudentsCount;
            var classrooms = _context.Classrooms.Where(c => c.Capacity >= classSize).ToList();
            var subjectTimes = _context.SubjectTimes.Where(s => s.StartDate >= selectedWeekDates.Item1 && s.StartDate < selectedWeekDates.Item2).ToList();

            var timeSlots = new TimeSpan[]
            {
                new TimeSpan(8, 0, 0),
                new TimeSpan(8, 55, 0),
                new TimeSpan(9, 50, 0),
                new TimeSpan(10, 35, 0),
                new TimeSpan(12, 00, 0),
                new TimeSpan(12, 45, 0)
            };

            // Identify working days (Monday to Friday) in the selected week
            List<DateTime> workingDays = Enumerable.Range(0, (selectedWeekDates.Item2 - selectedWeekDates.Item1).Days)
                                                    .Select(day => selectedWeekDates.Item1.AddDays(day))
                                                    .Where(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                                                    .ToList();
            int i = 0;
            int count = 0;
            DateTime currentWeekDay = selectedWeekDates.Item1;
            while (true)
            {
                bool added = false;
                //Console.WriteLine(Amounts[i]);
                if (Amounts.All(x => x == 0))
                    break;
                string subject = Subjects[i];
                string requiredClassroomType = ClassroomTypes[i];
                foreach (var classroom in classrooms.Where(c => c.UseCase == requiredClassroomType))
                {

                    foreach (var slot in timeSlots)
                    {

                        DateTime startTime = currentWeekDay + slot;
                        DateTime endTime = startTime.AddMinutes(45); // 45-minute lesson

                        bool isClassroomOccupied = subjectTimes.Any(st => st.FkClassroom == classroom.Id && st.StartDate >= startTime && st.StartDate < endTime);
                        bool isClassScheduled = subjectTimes.Any(st => st.StartDate >= startTime && st.StartDate < endTime && st.FkSchedule == schedule.Id);

                        if (!isClassroomOccupied && !isClassScheduled)
                        {
                            var newSubjectTime = new SubjectTime
                            {
                                Type = 1,
                                Descrtiption = subject,
                                FkSchedule = schedule.Id,
                                FkClassroom = classroom.Id,
                                FkSubject = subject,
                                StartDate = startTime,
                                EndDate = endTime
                            };

                            _context.SubjectTimes.Add(newSubjectTime);
                            subjectTimes.Add(newSubjectTime); // Update the list with the newly scheduled subject time
                            Amounts[i] = Amounts[i] - 1;
                            i++;
                            if (i >= Subjects.Count())
                                i = 0;
                            int c = 0;
                            while (Amounts[i] <= 0)
                            {
                                i++;
                                if (i >= Subjects.Count())
                                    i = 0;
                                c++;
                                if (c >= Subjects.Count())
                                    break;
                            }
                            if (currentWeekDay > selectedWeekDates.Item2)
                                currentWeekDay = selectedWeekDates.Item1;
                            else currentWeekDay = currentWeekDay.AddDays(1);
                            if (currentWeekDay.DayOfWeek == DayOfWeek.Sunday)
                                currentWeekDay = currentWeekDay.AddDays(1);
                            else if (currentWeekDay.DayOfWeek == DayOfWeek.Saturday)
                            {
                                currentWeekDay = currentWeekDay.AddDays(2);
                            }
                            if (currentWeekDay > selectedWeekDates.Item2)
                                currentWeekDay = selectedWeekDates.Item1;
                            added = true;
                            count = 0;
                            break;
                        }
                    }
                    if (added)
                    {
                        break;
                    }
                    
                }
                if (!added)
                {
                    count++;
                    if (currentWeekDay == selectedWeekDates.Item2)
                        currentWeekDay = selectedWeekDates.Item1;
                    else currentWeekDay = currentWeekDay.AddDays(1);
                    if (currentWeekDay.DayOfWeek == DayOfWeek.Sunday)
                        currentWeekDay = currentWeekDay.AddDays(1);
                    else if (currentWeekDay.DayOfWeek == DayOfWeek.Saturday)
                    {
                        currentWeekDay = currentWeekDay.AddDays(2);
                    }
                    if (currentWeekDay > selectedWeekDates.Item2)
                        currentWeekDay = selectedWeekDates.Item1;
                }
                if (count == 5)
                {
                    ScheduleFallbackLesson(schedule, subject, selectedWeekDates, subjectTimes);
                    Amounts[i] = Amounts[i] - 1;
                    i++;
                    if (i >= Subjects.Count())
                        i = 0;
                    int c = 0;
                    while (Amounts[i] <= 0)
                    {
                        i++;
                        if (i >= Subjects.Count())
                            i = 0;
                        c++;
                        if (c >= Subjects.Count())
                            break;
                    }
                    if (currentWeekDay > selectedWeekDates.Item2)
                        currentWeekDay = selectedWeekDates.Item1;
                    else currentWeekDay = currentWeekDay.AddDays(1);
                    if (currentWeekDay.DayOfWeek == DayOfWeek.Sunday)
                        currentWeekDay = currentWeekDay.AddDays(1);
                    else if (currentWeekDay.DayOfWeek == DayOfWeek.Saturday)
                    {
                        currentWeekDay = currentWeekDay.AddDays(2);
                    }
                    if (currentWeekDay > selectedWeekDates.Item2)
                        currentWeekDay = selectedWeekDates.Item1;
                    added = true;
                    count = 0;
                }
            }

            //Console.WriteLine("errrrrrrrrrrrr");

            _context.SaveChanges();
        }

        public void ScheduleFallbackLesson(Schedule schedule, string subjects, Tuple<DateTime, DateTime> selectedWeekDates, List<SubjectTime> subjectTimesfull)
        {
            
            var timeSlots = new TimeSpan[]
            {
                new TimeSpan(8, 0, 0),
                new TimeSpan(8, 55, 0),
                new TimeSpan(9, 50, 0),
                new TimeSpan(10, 35, 0),
                new TimeSpan(12, 00, 0),
                new TimeSpan(12, 45, 0)
            };

            // Identifying working days in the selected week
            List<DateTime> workingDays = Enumerable.Range(0, (selectedWeekDates.Item2 - selectedWeekDates.Item1).Days)
                                                    .Select(day => selectedWeekDates.Item1.AddDays(day))
                                                    .Where(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                                                    .ToList();

            // Get all existing SubjectTimes for the selected week
            var subjectTimes = subjectTimesfull.Where(s => s.StartDate >= selectedWeekDates.Item1 && s.StartDate < selectedWeekDates.Item2).ToList();
            bool added = false;
            foreach (var day in workingDays)
            {
                foreach (var slot in timeSlots)
                {
                    DateTime startTime = day + slot;
                    DateTime endTime = startTime.AddMinutes(45); // Assuming 45-minute lesson

                    bool isTimeSlotOccupied = subjectTimes.Any(st => st.StartDate >= startTime && st.StartDate < endTime && st.FkSchedule == schedule.Id);

                    if (!isTimeSlotOccupied)
                    {
                        var subject = subjects;
                        var newSubjectTime = new SubjectTime
                        {
                            Type = 2,
                            Descrtiption = subject,
                            FkSchedule = schedule.Id,
                            FkSubject = subject,
                            StartDate = startTime,
                            EndDate = endTime
                        };
                        _context.SubjectTimes.Add(newSubjectTime);
                        added = true;
                        break;
                    }
                }
                if (added)
                    break;
            }

            _context.SaveChanges();
        }




        public IActionResult GenerateSchedule()
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var subjects = _context.Subjects.ToList();
            var classes = _context.Classes.ToList();
            var classrooms = _context.Classrooms.ToList();
            ViewBag.classes = classes;
            ViewBag.classrooms = classrooms;
            return View(subjects);
        }

        public IActionResult EditSchedule(int scheduleId)
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            Schedule fullSchedule = _context.Schedules
                .Include(s => s.FkClassNavigation)
                .Include(s => s.SubjectTimes)
                .ThenInclude(st => st.FkSubjectNavigation)
                .SingleOrDefault(s => s.Id == scheduleId);
            fullSchedule.SubjectTimes = fullSchedule.SubjectTimes
                                    .OrderBy(st => st.StartDate)
                                    .ToList();
            //Check for conflicts
            var classes = _context.Classes.ToList();
            ViewBag.Classes = classes;
            return View(fullSchedule);
        }

        //[HttpPost]
        public IActionResult UpdateSchedule(Schedule model, DateTime SemestarStart, DateTime SemestarEnd, string ClassId)
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            model.SemesterStart = SemestarStart;
            model.SemesterEnd = SemestarEnd;
            model.FkClass = ClassId;
            if (!ValidateSchedule(model)) // Assuming ValidateSchedule is your custom validation method
            {
                // Add a model error
                ModelState.AddModelError("", "Nurodytame semestro laikotarpyje jau egzistuoja tvarkaraštis šiai klasei");

                // Prepare any additional data needed for the view
                ViewBag.Classes = _context.Classes.ToList();
                Schedule fullSchedule = _context.Schedules
                .Include(s => s.FkClassNavigation)
                .Include(s => s.SubjectTimes)
                .ThenInclude(st => st.FkSubjectNavigation)
                .SingleOrDefault(s => s.Id == model.Id);

                // Return the same view with the model and errors
                return View("EditSchedule", fullSchedule);
            }
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
            //Schedule fullSchedule = _context.Schedules
            //   .Include(s => s.FkClassNavigation)
            //   .Include(s => s.SubjectTimes)
            //   .ThenInclude(st => st.FkSubjectNavigation)
            //   .SingleOrDefault(s => s.Id == model.Id);
            return RedirectToAction("EditSchedule", new { scheduleId = model.Id });
        }

        public IActionResult EditLessonTime(int scheduleId, int LessonTimeid)
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
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
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
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
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
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
                if (!CheckIfClassRoomIsAvailable(model) || !CheckIfTimeSlotAvailable(model) || !CheckIfOutsideSemester(model))
                {
                    var subjectTime = _context.SubjectTimes.Find(model.Id);
                    var Classroom = _context.Classrooms.ToList();
                    var subjects = _context.Subjects.ToList();
                    var types = _context.SubjectTypes.ToList();
                    ViewBag.Classrooms = Classroom;
                    ViewBag.Subjects = subjects;
                    ViewBag.Types = types;
                    ViewBag.Schdeule = scheduleId;
                    return View("EditLessonTime", subjectTime);
                }
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
            }
            
            return RedirectToAction("EditSchedule", new { scheduleId = scheduleId });
        }

        public IActionResult AddLessonTime(int scheduleId)
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
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
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
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
                if (!CheckForUpcomingWeeksOfSemester(subjectTime))
                {
                    var Classroom = _context.Classrooms.ToList();
                    var subjects = _context.Subjects.ToList();
                    var types = _context.SubjectTypes.ToList();
                    ViewBag.Classrooms = Classroom;
                    ViewBag.Subjects = subjects;
                    ViewBag.Types = types;
                    return View("AddLessonTime", scheduleId);
                }
                AddForAllSemestarWeeks(subjectTime);
            }
            else
            {
                if (!CheckIfClassRoomIsAvailable(subjectTime) || !CheckIfTimeSlotAvailable(subjectTime) || !CheckIfOutsideSemester(subjectTime))
                {

                    var Classroom = _context.Classrooms.ToList();
                    var subjects = _context.Subjects.ToList();
                    var types = _context.SubjectTypes.ToList();
                    ViewBag.Classrooms = Classroom;
                    ViewBag.Subjects = subjects;
                    ViewBag.Types = types;
                    return View("AddLessonTime", scheduleId);
                }
                _context.SubjectTimes.Add(subjectTime);
                _context.SaveChanges();
            }
            return RedirectToAction("EditSchedule", new { scheduleId = scheduleId });
        }

       

        public IActionResult DeleteSchedule(int scheduleId)
        {
            if (HttpContext.Session.GetInt32("Permissions") != 10)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var schedule = _context.Schedules.Find(scheduleId);
            if (schedule != null)
            {
                var subjectTimes = _context.SubjectTimes.Where(s => s.FkSchedule == scheduleId).ToList();
                if (subjectTimes.Any())
                {
                    _context.SubjectTimes.RemoveRange(subjectTimes);
                }
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
            (s.SemesterEnd > schedule.SemesterStart && schedule.SemesterEnd > s.SemesterStart ) && schedule.Id != s.Id).ToList();
            if (schedules.Count == 0)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        public bool CheckIfClassRoomIsAvailable(SubjectTime subjecTime)
        {
            if (subjecTime.FkClassroom == null)
                return true;
            var subjectsTimes = _context.SubjectTimes.Where(s => !(s.StartDate > subjecTime.EndDate || s.EndDate < subjecTime.StartDate)
            && s.FkClassroom == subjecTime.FkClassroom && s.Id != subjecTime.Id).ToList();
            if (subjectsTimes.Any())
            {
                ModelState.AddModelError("", subjecTime.StartDate + " - " + subjecTime.EndDate + " Šiame laikotarpyje kabinetas yra užimtas");
                return false;
            }
            return true;
        }

        public bool CheckIfTimeSlotAvailable(SubjectTime subjecTime)
        {
            var subjectsTimes = _context.SubjectTimes.Where(s => !(s.StartDate > subjecTime.EndDate || s.EndDate < subjecTime.StartDate) && s.FkSchedule == subjecTime.FkSchedule && s.Id != subjecTime.Id ).ToList();
            if (subjectsTimes.Any())
            {
                ModelState.AddModelError("", subjecTime.StartDate + " - " + subjecTime.EndDate + " Šiame laikotarpyje šiai klasei jau vyksta pamoka");
                return false;
            }
            return true;
        }

        public bool CheckIfOutsideSemester(SubjectTime subjecTime)
        {
            Schedule schedule = _context.Schedules.Find(subjecTime.FkSchedule);
            if (subjecTime.StartDate < schedule.SemesterStart || subjecTime.EndDate > schedule.SemesterEnd)
            {
                ModelState.AddModelError("", subjecTime.StartDate + " - " + subjecTime.EndDate + " Šis laikotarpis patenka už semestro ribų");
                return false;
            }
            return true;
        }

        public bool CheckForUpcomingWeeksOfSemester(SubjectTime originalSubjectTime)
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
                    if (!CheckIfClassRoomIsAvailable(newSubjectTime) || !CheckIfTimeSlotAvailable(newSubjectTime) || !CheckIfOutsideSemester(newSubjectTime))
                    {
                        return false;
                    }


                }

                // Increment dates by 7 days
                startDate = startDate.AddDays(7);
                endDate = endDate.AddDays(7);
            }
            return true;
        }


    }
}
