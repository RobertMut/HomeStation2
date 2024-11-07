using System.ComponentModel.DataAnnotations;

namespace HomeStation.Application.Common.Enums;

public enum ReadingType
{
    [Display(Name = "Quality")]
    Quality,
    [Display(Name = "Temperature")]
    Climate,
    [Display(Name = "Complete")]
    Complete
}