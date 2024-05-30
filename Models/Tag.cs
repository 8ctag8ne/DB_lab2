using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DB_lab2;

public partial class Tag
{
    [Display(Name="ID")]
    public int TagId { get; set; }
    
    [Display(Name="Назва")]
    public string Name { get; set; } = null!;
    
    [Display(Name="Опис")]
    public string? Description { get; set; }
}
