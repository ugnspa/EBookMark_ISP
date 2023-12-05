using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class School
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<SchoolTeacher> SchoolTeachers { get; set; } = new List<SchoolTeacher>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
