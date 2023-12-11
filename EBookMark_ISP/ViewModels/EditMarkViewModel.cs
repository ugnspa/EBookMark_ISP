using EBookMark_ISP.Models;

namespace EBookMark_ISP.ViewModels
{
    public class EditMarkViewModel
    {
        public string subject { get; set; }
        public Student student { get; set; }
        public Mark Mark { get; set; }
        public List<SubjectTime> SubjectTimes { get; set; }
    }
}
