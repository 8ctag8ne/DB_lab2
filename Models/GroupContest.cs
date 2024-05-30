using System;
using System.Collections.Generic;

namespace DB_lab2;

public partial class GroupContest
{
    public int GroupId { get; set; }

    public int ContestId { get; set; }

    public virtual Contest Contest { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
