using HomeStation.Application.Common.Enums;
using HomeStation.Application.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HomeStation.Infrastructure.Persistence;

/// <summary>
/// Abstract DesignTimeDbContextFactoryBase
/// </summary>
/// <typeparam name="TContext">DbContext</typeparam>
public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
{
    private const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
    private DatabaseOptions? _databaseOptions;
    
    /// <summary>
    /// Create DbContext
    /// </summary>
    /// <param name="args"></param>
    /// <returns>Context</returns>
    public TContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory() + String.Format("{0}..{0}HomeStation.Web", Path.DirectorySeparatorChar);

        return Create(basePath, Environment.GetEnvironmentVariable(AspNetCoreEnvironment));
    }

    protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

    /// <summary>
    /// Creates instance based on conenction string
    /// </summary>
    /// <param name="basePath">current directory</param>
    /// <param name="environmentName">Environment name</param>
    /// <returns>Dbcontext</returns>
    private TContext Create(string basePath, string? environmentName)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.Local.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        _databaseOptions = configuration.GetSection("Database").Get<DatabaseOptions>();

        if (string.IsNullOrEmpty(_databaseOptions?.ConnectionString))
        {
            throw new Exception("Connection string is empty");
        }
            
        return Create(_databaseOptions.ConnectionString);
    }

    /// <summary>
    /// Gets instance of dbcontext from connection string
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>DbContext</returns>
    /// <exception cref="ArgumentException">If connection string is null or empty</exception>
    private TContext Create(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException($"Connection string is null or empty", nameof(connectionString));
        }

        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        
        switch (_databaseOptions?.DatabaseType)
        {
            case DatabaseType.SqlServer:
                optionsBuilder.UseSqlServer(connectionString);
                break;
            case DatabaseType.PostgreSql:
                optionsBuilder.UseNpgsql(connectionString);
                break;
            case DatabaseType.MySql:
                optionsBuilder.UseMySql(connectionString, ServerVersion.Parse(_databaseOptions.MySqlVersion));
                break;
        }

        return CreateNewInstance(optionsBuilder.Options);
    }
}