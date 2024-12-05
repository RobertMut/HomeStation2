using System.Diagnostics.CodeAnalysis;
using HomeStation.Application.CQRS.SaveReadingsCommand;
using HomeStation.Controllers;
using HomeStation.Domain.Common.Interfaces;
using Moq;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.AspNetCore.Routing;
using MQTTnet.Diagnostics;
using MQTTnet.Server;

namespace HomeStation.Web.Tests.Controllers;

[TestFixture]
[TestOf(typeof(MqttAirController))]
[ExcludeFromCodeCoverage]
public class MqttAirControllerTest
{
    private Mock<ICommandDispatcher> commandDispatcher;
    private MqttAirController controller;

    [SetUp]
    public void SetUp()
    {
        commandDispatcher = new Mock<ICommandDispatcher>();
        controller = new MqttAirController(commandDispatcher.Object);
    }
    
    [Test]
    public async Task ReadingsShouldDispatch()
    {
        commandDispatcher.Setup(x =>
                x.Dispatch(It.IsAny<SaveReadingsCommand>(), It.IsAny<CancellationToken>(), It.IsAny<string?>()))
            .Verifiable();
        
        controller.ControllerContext = new MqttControllerContext
        {
            MqttContext = new InterceptingPublishEventArgs(new MqttApplicationMessage(), CancellationToken.None, "clientID", new Dictionary<object, object>()),
            MqttServer = new MqttServer(new MqttServerOptions(), Enumerable.Empty<IMqttServerAdapter>(), new MqttNetEventLogger())
        };
        
        await controller.Readings(new SaveReadingsCommand()
        {
            Humidity = 1,
            Pm1_0 = 1
        });

        commandDispatcher.Verify(x =>
                x.Dispatch(It.Is<SaveReadingsCommand>(x => x.Humidity == 1.0d && x.Pm1_0 == 1),
                    It.IsAny<CancellationToken>(), It.Is<string?>(c => controller.ClientId == "clientID")),
            Times.Once);
    }
}