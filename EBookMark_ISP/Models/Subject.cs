using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Subject
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Language { get; set; } = null!;

    public int FkTeacher { get; set; }

    public virtual Teacher FkTeacherNavigation { get; set; } = null!;

    public virtual ICollection<Homework> Homeworks { get; set; } = new List<Homework>();

    public virtual ICollection<SubjectTime> SubjectTimes { get; set; } = new List<SubjectTime>();
}
