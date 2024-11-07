using System.ComponentModel.DataAnnotations;

namespace HomeStation.Application.Common.Enums;

public enum OperationType
{
    [Display(Name = "Approve")]
    Approve,
    
    [Display(Name = "Revoke")]
    Revoke
}