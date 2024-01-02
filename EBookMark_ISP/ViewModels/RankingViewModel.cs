using EBookMark_ISP.Models;
using System.Xml.Schema;

namespace EBookMark_ISP.ViewModels
{
    public class RankingViewModel
    {
        public Student student { get; set; }
        public List<StudentAverage> ThreeBetween { get; set; }
        public int total { get; set; }


    }
}
