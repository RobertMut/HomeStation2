using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeStation.Domain.Common.Entities;

/// <summary>
/// The air quality entity class
/// </summary>
public class Quality
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
    /// The PM2.5 readings
    /// </summary>
    public int Pm2_5 { get; set; }
    
    /// <summary>
    /// The PM10 readings
    /// </summary>
    public int Pm10 { get; set; }
    
    /// <summary>
    /// The PM1.0 readings
    /// </summary>
    public int Pm1_0 { get; set; }
    
    /// <summary>
    /// The readings dates
    /// </summary>
    public Reading Reading { get; set; }
    
    /// <summary>
    /// The device
    /// </summary>
    public Device Device { get; set; }
}