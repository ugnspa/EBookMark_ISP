using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Mark
{
    public int Id { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string Mark1 { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public int FkStudent { get; set; }

    public int FkSubjectTime { get; set; }

    public virtual Student FkStudentNavigation { get; set; } = null!;

    public virtual SubjectTime FkSubjectTimeNavigation { get; set; } = null!;
}
