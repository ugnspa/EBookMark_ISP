using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class File
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Size { get; set; }

    public int FkHomework { get; set; }

    public virtual Homework FkHomeworkNavigation { get; set; } = null!;
}
