using EBookMark_ISP.Models;
namespace EBookMark_ISP.ViewModels
{
    public class GradeBookViewModel
    {
        public List<string> teachers_subject { get; set; }
        public Student student { get; set; }
        public Dictionary<Schedule, List<SubjectMarks>> schedules { get; set;}


        public class SubjectMarks
        {
            public string subject { get; set; }
            public List<MarkTime> marksTimes { get; set; }

            public double average()
            {
                var validMarks = marksTimes
                            .Where(mt => int.TryParse(mt.mark.Mark1, out _))
                            .Select(mt => int.Parse(mt.mark.Mark1));

                if (!validMarks.Any())
                {
                    return 0;
                }

                return validMarks.Average();
            }
        }
        public class MarkTime
        {
            public DateTime time { get; set; }
            public Mark mark { get; set; }
        }
    }
}
