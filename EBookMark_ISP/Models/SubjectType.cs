using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class SubjectType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<SubjectTime> SubjectTimes { get; set; } = new List<SubjectTime>();
}
