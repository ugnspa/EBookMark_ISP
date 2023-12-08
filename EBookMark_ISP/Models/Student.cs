using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Student
{
    public string PersonalCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public bool Document { get; set; }

    public int Gender { get; set; }

    public int FkUser { get; set; }

    public int FkSchool { get; set; }

    public string? FkClass { get; set; }

    public int FkGuardian { get; set; }

    public virtual Class? FkClassNavigation { get; set; }

    public virtual Guardian FkGuardianNavigation { get; set; } = null!;

    public virtual School FkSchoolNavigation { get; set; } = null!;

    public virtual User FkUserNavigation { get; set; } = null!;

    public virtual Gender GenderNavigation { get; set; } = null!;

    public virtual ICollection<Mark> Marks { get; set; } = new List<Mark>();
}
