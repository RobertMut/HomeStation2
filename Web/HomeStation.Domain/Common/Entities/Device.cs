namespace HomeStation.Domain.Common.Entities;

/// <summary>
/// The device entity class
/// </summary>
public class Device
{
    /// <summary>
    /// The unique identifier 
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The device name
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The device known flag
    /// </summary>
    public bool IsKnown { get; set; }
    
    /// <summary>
    /// Climate readings
    /// </summary>
    public IEnumerable<Climate> Climate { get; set; }
    
    /// <summary>
    /// The air quality readings
    /// </summary>
    public IEnumerable<Quality> AirQuality { get; set; }
}