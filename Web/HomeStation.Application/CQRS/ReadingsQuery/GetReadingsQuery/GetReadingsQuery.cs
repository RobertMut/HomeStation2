using HomeStation.Application.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeStation.Application.CQRS.ReadingsQuery.GetReadingsQuery;

public class GetReadingsQuery
{
    public int DeviceId { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public ReadingType ReadingType { get; set; } = ReadingType.Complete;
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTimeOffset StartDate { get; set; }
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTimeOffset EndDate { get; set; }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public DetailLevel DetailLevel { get; set; } = DetailLevel.Normal;
}