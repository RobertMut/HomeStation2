using System.ComponentModel.DataAnnotations;

namespace HomeStation.Application.Common.Enums;

public enum DetailLevel
{
    [Display(Name = "Detailed")]
    Detailed,
    
    [Display(Name = "Normal")]
    Normal,
    
    [Display(Name = "Less")]
    Less
}