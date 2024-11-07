using System.Net;
using HomeStation.Application.Common.Enums;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Options;
using HomeStation.Infrastructure.Helpers;
using HomeStation.Infrastructure.Persistence;
using HomeStation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet.AspNetCore;

namespace HomeStation.Infrastructure;

public static partial class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        DatabaseOptions? databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        MQTTOptions mqttOptions = serviceProvider.GetRequiredService<IOptions<MQTTOptions>>().Value;
        
        services.AddDbContext<AirDbContext>(options =>
        {
            switch (databaseOptions?.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    options.UseSqlServer(databaseOptions.ConnectionString);
                    break;
                case DatabaseType.PostgreSql:
                    options.UseSqlServer(databaseOptions.ConnectionString);
                    break;
                case DatabaseType.MySql:
                    options.UseNpgsql(databaseOptions.ConnectionString);
                    break;
                default:
                    throw new NotSupportedException($"Database type {databaseOptions.DatabaseType} not supported");
            }
        });

        services.AddScoped<IAirDbContext>(provider => provider.GetRequiredService<AirDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        await DatabaseHelper.InitDatabase(databaseOptions, services);
        
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
}