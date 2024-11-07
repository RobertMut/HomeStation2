using HomeStation.Domain.Common.Interfaces;
using Newtonsoft.Json;

namespace HomeStation.Application.CQRS.SaveReadingsCommand;

public class SaveReadingsCommand : ICommand
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double Pressure { get; set; }
    
    public int Pm1_0 { get; set; }
    public int Pm2_5 { get; set; }
    public int Pm10 { get; set; }
}