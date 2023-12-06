﻿using EBookMark_ISP.Models;
namespace EBookMark_ISP.ViewModels
{
    public class GradeBookViewModel
    {
        public Student student { get; set; }
        public Dictionary<Schedule, List<SubjectMarks>> schedules { get; set;}


        public class SubjectMarks
        {
            public string subject { get; set; }
            public List<MarkTime> marksTimes { get; set; }
        }
        public class MarkTime
        {
            public DateTime time { get; set; }
            public Mark mark { get; set; }
        }
    }
}