namespace HomeStation.Application.Common.Options;

public class MqttOptions
{
    public const string MQTT = "MQTT";
    
    public string Address { get; set; }
    public int Port { get; set; }
}