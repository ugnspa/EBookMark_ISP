using System.Globalization;

namespace EBookMark_ISP.Models
{
    public class LessonTime
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public string Subject { get; set; }

        public Classroom room { get; set; }

        public LessonTime()
        {
            // Initialize a new Classroom if necessary
            this.room = new Classroom();
        }

        public LessonTime(int id, DateTime start, DateTime end, string desc, string type, string subject, Classroom room = null)
        {
            Id = id;
            Start = start;
            End = end;
            Description = desc;
            Type = type;
            this.room = room;
            this.Subject = subject;
        }
    }
}
