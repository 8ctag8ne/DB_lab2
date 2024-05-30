using System;
using System.Collections.Generic;

namespace DB_lab2;

public partial class ContestProblem
{
    public int ProblemId { get; set; }

    public int ContestId { get; set; }

    public virtual Contest Contest { get; set; } = null!;

    public virtual Problem Problem { get; set; } = null!;
}
