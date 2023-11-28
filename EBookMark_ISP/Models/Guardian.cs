using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Guardian
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
