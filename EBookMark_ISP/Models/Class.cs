using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Class
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int StudentsCount { get; set; }

    public int Year { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
