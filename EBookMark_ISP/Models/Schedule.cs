using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Schedule
{
    public int Id { get; set; }

    public DateTime SemesterStart { get; set; }

    public DateTime SemesterEnd { get; set; }

    public string FkClass { get; set; } = null!;

    public virtual Class FkClassNavigation { get; set; } = null!;

    public virtual ICollection<SubjectTime> SubjectTimes { get; set; } = new List<SubjectTime>();
}
