using System.Diagnostics.CodeAnalysis;
using HomeStation.Application;
using HomeStation.Application.Common.Options;
using HomeStation.Infrastructure;
using HomeStation.Middleware;
using Microsoft.Extensions.Options;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Routing;
using MQTTnet.Server;

namespace HomeStation;

[ExcludeFromCodeCoverage]
internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.Database));
        builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection(MqttOptions.MQTT));

        builder.Services.AddExceptionHandler<ApiExceptionHandler>();
        await builder.Services.AddInfrastructure();
        builder.Services.AddApplication();

        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddMqttControllers();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();
        app.MapMqtt("/mqtt");

        app.UseMqttServer(server =>
        {
            server.WithAttributeRouting(app.Services);
            ILogger<MqttServer>? logger = app.Services.GetService<ILogger<MqttServer>>();
            IOptions<MqttOptions> options = app.Services.GetRequiredService<IOptions<MqttOptions>>();
    
            server.StartedAsync += eventArgs =>
            {
                logger.LogInformation("MQTT started at port {Port}", options.Value.Port);
                return Task.CompletedTask;
            };
            server.ClientConnectedAsync += eventArgs =>
            {
                logger.LogInformation("User connected: {ClientId}", eventArgs.ClientId);
                return Task.CompletedTask;
            };
            server.AcceptNewConnections = true;
            server.StartAsync();
        });

        app.MapFallbackToFile("/index.html", new StaticFileOptions()
        {
            ServeUnknownFileTypes = true
        });

        app.Run();
    }
}