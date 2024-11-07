using HomeStation.Application.Common.Enums;
using HomeStation.Domain.Common.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeStation.Application.CQRS.ApproveRevokeDeviceCommand;

public class ApproveRevokeDeviceCommand : ICommand
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public OperationType Operation { get; set; }
}