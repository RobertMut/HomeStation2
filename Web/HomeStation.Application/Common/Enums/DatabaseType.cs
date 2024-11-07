using System.ComponentModel.DataAnnotations;

namespace HomeStation.Application.Common.Enums;

public enum DatabaseType
{
    [Display(Name = "SqlServer")]
    SqlServer,
    
    [Display(Name = "PostgreSql")]
    PostgreSql,
    
    [Display(Name = "MySql")]
    MySql
}