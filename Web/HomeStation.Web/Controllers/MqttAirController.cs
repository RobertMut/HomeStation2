using HomeStation.Application.CQRS;
using HomeStation.Application.CQRS.SaveReadingsCommand;
using HomeStation.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MQTTnet.AspNetCore.Routing;
using MQTTnet.AspNetCore.Routing.Attributes;

namespace HomeStation.Controllers;

[MqttController]
[MqttRoute("air")]
public class MqttAirController : MqttBaseController
{
    private readonly ICommandDispatcher _commandDispatcher;

    public MqttAirController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [MqttRoute("readings")]
    public async Task Readings([FromPayload] SaveReadingsCommand command)
    {
        await _commandDispatcher.Dispatch(command, new CancellationToken(), ClientId);
    }
}