using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DB_lab2;

public partial class User
{
    [Display(Name="ID")]
    public int UserId { get; set; }

    [Display(Name="Логін")]
    public string Login { get; set; } = null!;

    [Display(Name="Пароль")]
    public string PasswordHash { get; set; } = null!;

    [Display(Name="Рейтинг")]
    public int? Rating { get; set; }

    [Display(Name="Адміністратор")]
    public bool IsAdmin { get; set; } = false;

    public virtual ICollection<Contest> Contests { get; set; } = new List<Contest>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Problem> Problems { get; set; } = new List<Problem>();

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
