using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DB_lab2;

public partial class Submission
{
    [Display(Name="ID")]
    public int SubmissionId { get; set; }

    [Display(Name="Група")]
    public int? GroupId { get; set; }

    [Display(Name="Змагання")]
    public int? ContestId { get; set; }

    [Display(Name="Автор")]
    public int? SubmittedBy { get; set; }

    [Display(Name="Задача")]
    public int? ProblemId { get; set; }

    [Display(Name="Кількість балів")]
    public int? Result { get; set; }

    [Display(Name="Вердикт")]
    public string? Verdict { get; set; }

    [Display(Name="Час відправки")]
    public DateTime? SubmittedAt { get; set; }

    [Display(Name="Код")]
    public string? Code { get; set; }

    [Display(Name="Мова")]
    public string? Language { get; set; }

    [Display(Name="Змагання")]
    public virtual Contest? Contest { get; set; }

    [Display(Name="Група")]
    public virtual Group? Group { get; set; }
    
    [Display(Name="Задача")]
    public virtual Problem? Problem { get; set; }

    [Display(Name="Автор")]
    public virtual User? SubmittedByNavigation { get; set; }
}
