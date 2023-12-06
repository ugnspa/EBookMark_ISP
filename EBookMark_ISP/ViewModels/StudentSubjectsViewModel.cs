using EBookMark_ISP.Models;
namespace EBookMark_ISP.ViewModels
{
    public class StudentSubjectsViewModel
    {
        public Student student { get; set; }
        public Dictionary<Schedule,List<Subject>> ScheduleSubjects { get; set; }
    }
}
