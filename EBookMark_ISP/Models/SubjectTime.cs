using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class SubjectTime
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Descrtiption { get; set; } = null!;

    public int Type { get; set; }

    public int? FkClassroom { get; set; }

    public string FkSubject { get; set; } = null!;

    public int FkSchedule { get; set; }

    public virtual Classroom? FkClassroomNavigation { get; set; }

    public virtual Schedule FkScheduleNavigation { get; set; } = null!;

    public virtual Subject FkSubjectNavigation { get; set; } = null!;

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();

    public virtual SubjectType TypeNavigation { get; set; } = null!;
}
