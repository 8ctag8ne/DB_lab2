using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DB_lab2;

public partial class Group
{
    [Display(Name="ID")]
    public int GroupId { get; set; }

    [Display(Name="Назва")]
    [Required(ErrorMessage ="Введіть назву групи")]
    public string Name { get; set; } = null!;

    [Display(Name="Опис")]
    public string? Description { get; set; }

    [Display(Name="Автор")]
    public int? CreatedBy { get; set; }

    [Display(Name="Автор")]
    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
