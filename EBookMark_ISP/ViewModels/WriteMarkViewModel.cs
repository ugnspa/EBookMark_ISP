using EBookMark_ISP.Models;
namespace EBookMark_ISP.ViewModels
{
    public class WriteMarkViewModel
    {
        public Student student { get; set; }
        public Subject subject { get; set; }

        public List<SubjectTime> subjectTimes { get; set; }
    }
}
