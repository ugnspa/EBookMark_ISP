using EBookMark_ISP.Models;

namespace EBookMark_ISP.ViewModels
{
    public class FilterViewModel
    {
        public Student student { get; set; }
        public Dictionary<Schedule, List<string>> schedule_subjects { get; set; }
        public List<Gender> genders { get; set; }
        public List<string> scales { get; set; }
    }
}
