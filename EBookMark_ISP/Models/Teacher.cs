using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Teacher
{
    public string PersonalCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime EmploymentDate { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public int Gender { get; set; }

    public int FkUser { get; set; }

    public virtual User FkUserNavigation { get; set; } = null!;

    public virtual Gender GenderNavigation { get; set; } = null!;

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
