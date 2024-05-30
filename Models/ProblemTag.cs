using System;
using System.Collections.Generic;

namespace DB_lab2;

public partial class ProblemTag
{
    public int ProblemId { get; set; }

    public int TagId { get; set; }

    public virtual Problem Problem { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
