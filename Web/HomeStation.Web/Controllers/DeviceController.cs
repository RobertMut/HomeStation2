using HomeStation.Application.CQRS.ApproveRevokeDeviceCommand;
using HomeStation.Application.CQRS.GetDevicesQuery;
using HomeStation.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public DeviceController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        [HttpGet]
        public async Task<IEnumerable<DeviceWebModel>> GetDevices()
        {
            return await _queryDispatcher.Dispatch<GetDevicesQuery, IEnumerable<DeviceWebModel>>(new GetDevicesQuery(),
                new CancellationToken());
        }

        [HttpPut("approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveRevokeDeviceCommand command)
        {
            await _commandDispatcher.Dispatch(command, new CancellationToken());

            return Ok();
        }
    }
}
