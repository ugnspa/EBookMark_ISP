using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class SchoolTeacher
{
    public int FkSchool { get; set; }

    public int FkTeacher { get; set; }

    public virtual School FkSchoolNavigation { get; set; } = null!;
}
