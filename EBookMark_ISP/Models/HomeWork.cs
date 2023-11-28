using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Homework
{
    public int Id { get; set; }

    public DateTime DueToDate { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public DateTime UploadDate { get; set; }

    public int FilesCount { get; set; }

    public string FkSubject { get; set; } = null!;

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual Subject FkSubjectNavigation { get; set; } = null!;
}
