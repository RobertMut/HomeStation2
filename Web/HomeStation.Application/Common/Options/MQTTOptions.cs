using HomeStation.Application.Common.Enums;

namespace HomeStation.Application.Common.Options;

public class MQTTOptions
{
    public const string MQTT = "MQTT";
    
    public string Address { get; set; }
    public int Port { get; set; }
}