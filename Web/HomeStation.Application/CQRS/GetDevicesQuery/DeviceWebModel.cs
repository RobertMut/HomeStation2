namespace HomeStation.Application.CQRS.GetDevicesQuery;

public class DeviceWebModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsKnown { get; set; }
}