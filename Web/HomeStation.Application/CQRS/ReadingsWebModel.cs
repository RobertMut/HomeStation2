namespace HomeStation.Application.CQRS;

public class ReadingsWebModel
{
    public int DeviceId { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public double? Pressure { get; set; }
    public int? Pm1_0 { get; set; }
    public int? Pm2_5 { get; set; }
    public int? Pm10 { get; set; }
    public DateTimeOffset ReadDate { get; set; }
}