namespace EBookMark_ISP.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public DateTime SemestarStart { get; set; }
        public DateTime SemestarEnd { get; set; }

        public string className { get; set; }

        public List<LessonTime> Lessons;

        public Schedule()
        {
        }

        public Schedule(int id, DateTime semestarStart, DateTime semestarEnd, string className)
        {
            ScheduleId = id;
            SemestarStart = semestarStart;
            SemestarEnd = semestarEnd;
            this.className = className;
            Lessons = new List<LessonTime>();
        }

        public void Add(LessonTime time)
        {
            Lessons.Add(time);
        }
    }
}
