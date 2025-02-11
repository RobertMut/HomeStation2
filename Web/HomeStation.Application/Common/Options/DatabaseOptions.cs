using HomeStation.Application.Common.Enums;

namespace HomeStation.Application.Common.Options;

public class DatabaseOptions
{
    public const string Database = "Database";
    
    public string ConnectionString { get; set; }
}