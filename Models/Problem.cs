using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DB_lab2;

public partial class Problem
{
    [Display(Name="ID")]
    public int ProblemId { get; set; }

    [Display(Name="Назва")]
    public string Name { get; set; } = null!;

    [Display(Name="Опис")]
    public string? Description { get; set; }

    [Display(Name="Складність")]
    public int? DifficultyLevel { get; set; }

    [Display(Name="Автор")]
    public int? CreatedBy { get; set; }

    [Display(Name="Розбір")]
    public string? Editorial { get; set; }

    [Display(Name="Тести")]
    public string? TestFile { get; set; }

    [Display(Name="Автор")]
    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
