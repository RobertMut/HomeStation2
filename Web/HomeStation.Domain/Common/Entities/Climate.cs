using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStation.Domain.Common.Entities;

/// <summary>
/// The Climate Entity Class
/// </summary>
public class Climate
{
    /// <summary>
    /// The unique identifier
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The device id
    /// </summary>
    public int DeviceId { get; set; }

    /// <summary>
    /// The temperature
    /// </summary>
    public double Temperature { get; set; }
    
    /// <summary>
    /// The humidity
    /// </summary>
    public double Humidity { get; set; }
    
    /// <summary>
    /// The pressure
    /// </summary>
    public double Pressure { get; set; }
    
    /// <summary>
    /// The reading dates
    /// </summary>
    public Reading Reading { get; set; }
    
    /// <summary>
    /// The device
    /// </summary>
    public Device Device { get; set; }
}