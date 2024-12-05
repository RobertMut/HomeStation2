using HomeStation.Application.Common.Enums;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Options;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace HomeStation.Infrastructure.Helpers;

/// <summary>
/// The database helper class
/// </summary>
public class DatabaseHelper
{
    private delegate Task<int> GetTableCount(DatabaseOptions options);
    
    private const string MySqlCheck = @"SELECT count(*)
                                    FROM information_schema.tables
                                    WHERE table_name LIKE '%Climate%' OR table_name LIKE '%Devices%'";

    private const string SqlServerCheck = @"SELECT COUNT(*)
                                            FROM INFORMATION_SCHEMA.TABLES
                                            WHERE TABLE_NAME LIKE '%Climate%' OR TABLE_NAME LIKE '%Devices%'";
    
    /// <summary>
    /// Inits new database if doesn't exist
    /// </summary>
    /// <param name="options">The <see cref="DatabaseOptions"/></param>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/></param>
    /// <exception cref="NotImplementedException">If PostgreSql, not supported for now</exception>
    public static async Task InitDatabase(DatabaseOptions options, IServiceCollection serviceCollection)
    {
        IAirDbContext dbContext = serviceCollection.BuildServiceProvider().GetRequiredService<IAirDbContext>();
        GetTableCount tableCount;
        
        switch (options.DatabaseType)
        {
            case DatabaseType.MySql:
                tableCount = GetMySqlTableCount;
                break;
            
            case DatabaseType.SqlServer:
                tableCount = GetSqlServerTableCount;
                break;
            
            default:
            case DatabaseType.PostgreSql:
                throw new NotImplementedException("Currently not supported");
        }

        int count = await ExecuteSafely(tableCount, options);
        await Migrate(count, dbContext);
    }

    /// <summary>
    /// Executes method safely
    /// </summary>
    /// <param name="method">The table count method delegate.</param>
    /// <param name="options">The <see cref="DatabaseOptions"/></param>
    /// <returns>Count of tables in database</returns>
    private static async Task<int> ExecuteSafely(GetTableCount method, DatabaseOptions options)
    {
        try
        {
            return await method(options);
        }
        catch (SqlException)
        {
            return 0;
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
    }
    
    /// <summary>
    /// Gets mysql table count
    /// </summary>
    /// <param name="options">The <see cref="DatabaseOptions"/></param>
    /// <returns>Table count</returns>
    private static async Task<int>GetMySqlTableCount(DatabaseOptions options)
    {
        int count;
        await using (var connection = new MySqlConnection { ConnectionString = options.ConnectionString })
        await using (MySqlCommand sqlCommand = new MySqlCommand(MySqlCheck, connection))
        {
            await connection.OpenAsync();
            count = Convert.ToInt32(await sqlCommand.ExecuteScalarAsync());

            await connection.CloseAsync();
        }

        return count;
    } 
    
    /// <summary>
    /// Gets sql server table count
    /// </summary>
    /// <param name="options"><see cref="DatabaseOptions"/></param>
    /// <returns>Table count</returns>
    private static async Task<int>GetSqlServerTableCount(DatabaseOptions options)
    {
        int count;
        
        await using (SqlConnection connection = new SqlConnection(options.ConnectionString))
        await using (SqlCommand command = new SqlCommand(SqlServerCheck, connection))
        {
            await connection.OpenAsync();
            count = Convert.ToInt32(await command.ExecuteScalarAsync());
            await connection.CloseAsync();
        }

        return count;
    } 
    
    /// <summary>
    /// Migrates database if table known count is 0
    /// </summary>
    /// <param name="count">The known table count</param>
    /// <param name="dbContext">The <see cref="IAirDbContext"/></param>
    private static async Task Migrate(int count, IAirDbContext dbContext)
    {
        if (count == 0)
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}