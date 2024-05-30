using System;
using System.Collections.Generic;

namespace DB_lab2;

public partial class GroupUser
{
    public int GroupId { get; set; }

    public int UserId { get; set; }

    public bool IsAdmin { get; set; } = false;

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
