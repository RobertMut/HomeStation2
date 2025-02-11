using System.Data;
using System.Diagnostics.CodeAnalysis;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Options;
using HomeStation.Infrastructure.Persistence;
using HomeStation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet.AspNetCore;

namespace HomeStation.Infrastructure;

[ExcludeFromCodeCoverage]
public static partial class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        DatabaseOptions? databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        MqttOptions mqttOptions = serviceProvider.GetRequiredService<IOptions<MqttOptions>>().Value;
        
        services.AddDbContext<AirDbContext>(options =>
        {
            options.UseSqlServer(databaseOptions.ConnectionString,
                b => b.MigrationsAssembly(typeof(AirDbContext).Assembly.FullName)
                    .EnableRetryOnFailure(5, TimeSpan.FromMilliseconds(5000), []));
        });

        services.AddScoped<IAirDbContext>(provider => provider.GetRequiredService<AirDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        await MigrateAsync(services.BuildServiceProvider());
        
        services.AddHostedMqttServerWithServices(configure =>
            {
                configure.WithDefaultEndpoint();
                configure.WithDefaultEndpointPort(mqttOptions.Port);
            })
            .AddMqttConnectionHandler()
            .AddConnections()
            .AddMqttTcpServerAdapter();
        
        return services;
    }
    
    private static async Task MigrateAsync(IServiceProvider serviceProvider)
    {
        List<Exception> exceptions = new List<Exception>();
        int maxRetryCount = 5;
        
        using IServiceScope scope = serviceProvider.CreateScope();
        AirDbContext context = scope.ServiceProvider.GetRequiredService<AirDbContext>();
        IEnumerable<string> migrations = await context.Database.GetPendingMigrationsAsync();
        string[] migrationsArr = migrations as string[] ?? migrations.ToArray();
        if (!migrationsArr.Any()) return;
        
        foreach (string migration in migrationsArr)
        {
            await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            
            int retryCount = 0;
            bool isSuccess = false;
            
            while (retryCount <= maxRetryCount || isSuccess)
            {
                isSuccess = await MigrationTransaction(migration, context.Database, transaction, exceptions);
            }
        }

        if (exceptions.Any())
        {
            throw new AggregateException("An error occurred while migrating the database", exceptions);
        }
    }

    private static async Task<bool> MigrationTransaction(string migrationName, DatabaseFacade facade, IDbContextTransaction transaction,
        List<Exception> exceptions)
    {
        try
        {
            await facade.MigrateAsync(migrationName);
            await transaction.CommitAsync();
            
            return true;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            exceptions.Add(e);
            
            return false;
        }
    }
}