using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
public class DatabaseHelper //todo refactor in future
{
    private const string MySqlCheck = @"SELECT count(*)
                                    FROM information_schema.tables
                                    WHERE table_name LIKE '%Climate%' OR table_name LIKE '%Devices%'";

    private const string SqlServerCheck = @"SELECT COUNT(*)
                                            FROM   INFORMATION_SCHEMA.TABLES
                                            WHERE  TABLE_NAME LIKE '%Climate%' OR TABLE_NAME LIKE '%Devices%'";
    
    /// <summary>
    /// Inits new database if doesn't exist
    /// </summary>
    /// <param name="options">The <see cref="DatabaseOptions"/></param>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/></param>
    /// <exception cref="NotImplementedException">If PostgreSql, not supported for now</exception>
    public static async Task InitDatabase(DatabaseOptions options, IServiceCollection serviceCollection)
    {
        int count = 0;
        IAirDbContext dbContext = serviceCollection.BuildServiceProvider().GetRequiredService<IAirDbContext>();
        switch (options.DatabaseType)
        {
            case DatabaseType.MySql:
                try
                {
                    await using (var connection = new MySqlConnection
                                 {
                                     ConnectionString = options.ConnectionString
                                 })
                    await using (MySqlCommand sqlCommand = new MySqlCommand(MySqlCheck, connection))
                    {
                        await connection.OpenAsync();
                        count = Convert.ToInt32(sqlCommand.ExecuteScalar());

                        await connection.CloseAsync();
                    }
                }
                catch (InvalidOperationException)
                {
                    count = 0;
                }

                await Migrate(count, dbContext);
                
                break;
            
            case DatabaseType.SqlServer:

                try
                {
                    await using (SqlConnection connection = new SqlConnection(options.ConnectionString))
                    await using (SqlCommand command = new SqlCommand(SqlServerCheck, connection))
                    {
                        await connection.OpenAsync();
                        count = Convert.ToInt32(command.ExecuteScalar());
                        await connection.CloseAsync();
                    }
                }
                catch (SqlException)
                {
                    count = 0;
                }

                await Migrate(count, dbContext);
                
                break;
            
            case DatabaseType.PostgreSql:
                throw new NotImplementedException("Currently not supported");
        }
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

            string sql = dbContext.Database.GenerateCreateScript();
            sql = Regex.Replace(sql, @"(\r\n|\n\r|\n|\r)", "\n");
            
            var statements = Regex.Split(
                sql,
                @"^[\t ]*GO[\t ]*\d*[\t ]*(?:--.*)?$",
                RegexOptions.Multiline |
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.IgnoreCase);

            foreach (var statement in statements)
            {
                await dbContext.Database.ExecuteSqlAsync(FormattableStringFactory.Create(statement));
            }
        }
    }
}