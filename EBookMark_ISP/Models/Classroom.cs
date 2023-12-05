using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Classroom
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string UseCase { get; set; } = null!;

    public int Floor { get; set; }

    public int Capacity { get; set; }

    public string Building { get; set; } = null!;

    public virtual ICollection<SubjectTime> SubjectTimes { get; set; } = new List<SubjectTime>();
}
