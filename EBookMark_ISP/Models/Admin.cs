using System;
using System.Collections.Generic;

namespace EBookMark_ISP.Models;

public partial class Admin
{
    public int FkUser { get; set; }

    public virtual User FkUserNavigation { get; set; } = null!;
}
