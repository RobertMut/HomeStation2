using HomeStation.Application.Common.Enums;
using HomeStation.Application.Common.Exceptions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Domain.Common.Entities;
using HomeStation.Domain.Common.Interfaces;

namespace HomeStation.Application.CQRS.ReadingsQuery;

/// <summary>
/// The class for getting readings
/// </summary>
public class GetReadingsQueryHandler : IQueryHandler<GetReadingsQuery, IEnumerable<ReadingsWebModel>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <inheritdoc cref="IQueryHandler{GetReadingsQuery,IEnumerable}"/>
    public GetReadingsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles getting readings
    /// </summary>
    /// <param name="query">The <see cref="GetReadingsQuery"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IEnumerable{ReadingsWebModel}"/> of <see cref="ReadingsWebModel"/>.</returns>
    /// <exception cref="Exception">If device not found.</exception>
    public async Task<IEnumerable<ReadingsWebModel>?> Handle(GetReadingsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.StartDate == query.EndDate)
        {
            query.EndDate = query.EndDate.AddDays(1).AddTicks(-1);
        }

        Device device = await GetData(query, cancellationToken);

        IEnumerable<ReadingsWebModel>? readings = GetReadings(query, device.Climate, device.AirQuality);

        return LimitDataCount(readings, query.DetailLevel)?.OrderBy(d => d.ReadDate);
    }

    /// <summary>
    /// Gets data from database
    /// </summary>
    /// <param name="query">The <see cref="GetReadingsQuery"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Device"/> with readings</returns>
    /// <exception cref="ArgumentOutOfRangeException">If unknown reading type.</exception>
    private async Task<Device> GetData(GetReadingsQuery query, CancellationToken cancellationToken)
    {
        using IUnitOfWork unitOfWork = _unitOfWork;
        Device device = await unitOfWork.DeviceRepository.GetObjectBy(x => x.Id == query.DeviceId,
            cancellationToken: cancellationToken);

        if (device == null)
        {
            throw new NotFoundException("Device not found");
        }

        IEnumerable<Climate> climate = unitOfWork.ClimateRepository
            .Get(q => q.Where(c => c.DeviceId == device.Id
                                   && (query.StartDate.Year <= c.Reading.Date.Year &&
                                       query.StartDate.Month <= c.Reading.Date.Month &&
                                       query.StartDate.Day <= c.Reading.Date.Day) &&
                                   (c.Reading.Date.Year <= query.EndDate.Year &&
                                    c.Reading.Date.Month <= query.EndDate.Month &&
                                    c.Reading.Date.Day <= query.EndDate.Day)))
            .AsEnumerable();
        IEnumerable<Quality> quality = unitOfWork.QualityRepository
            .Get(q => q.Where(q => q.DeviceId == device.Id
                                   && (query.StartDate.Year <= q.Reading.Date.Year &&
                                       query.StartDate.Month <= q.Reading.Date.Month &&
                                       query.StartDate.Day <= q.Reading.Date.Day) &&
                                   (q.Reading.Date.Year <= query.EndDate.Year &&
                                    q.Reading.Date.Month <= query.EndDate.Month &&
                                    q.Reading.Date.Day <= query.EndDate.Day)))
            .AsEnumerable();

        device.AirQuality = quality.ToList();
        device.Climate = climate.ToList();

        return device;
    }

    /// <summary>
    /// Converts database readings to web model
    /// </summary>
    /// <param name="query">The <see cref="GetReadingsQuery"/>.</param>
    /// <param name="climateReadings">The optional <see cref="IEnumerable{Climate}"/> of <see cref="Climate"/>.</param>
    /// <param name="airQualityReadings">The optional <see cref="IEnumerable{Quality}"/> of <see cref="Quality"/>.</param>
    /// <returns>The <see cref="IEnumerable{ReadingsWebModel}"/> of <see cref="ReadingsWebModel"/>.</returns>
    private static IEnumerable<ReadingsWebModel>? GetReadings(GetReadingsQuery query,
        IEnumerable<Climate>? climateReadings, IEnumerable<Quality>? airQualityReadings)
    {
        if (climateReadings == null)
        {
            return airQualityReadings.Select(x => new ReadingsWebModel
            {
                DeviceId = query.DeviceId,
                Pm1_0 = x.Pm1_0,
                Pm2_5 = x.Pm2_5,
                Pm10 = x.Pm10,
                ReadDate = x.Reading.Date
            });
        }

        if (airQualityReadings == null)
        {
            return climateReadings.Select(x => new ReadingsWebModel
            {
                DeviceId = query.DeviceId,
                Temperature = x.Temperature,
                Humidity = x.Humidity,
                Pressure = x.Pressure,
                ReadDate = x.Reading.Date
            });
        }

        return from climate in climateReadings
            join air in airQualityReadings
                on climate?.DeviceId equals air?.DeviceId
            select
                new ReadingsWebModel
                {
                    DeviceId = query.DeviceId,
                    Temperature = climate?.Temperature,
                    Humidity = climate?.Humidity,
                    Pressure = climate?.Pressure,
                    Pm1_0 = air?.Pm1_0,
                    Pm2_5 = air?.Pm2_5,
                    Pm10 = air?.Pm10,
                    ReadDate = air.Reading.Date
                };
    }

    /// <summary>
    /// Limits the returned data
    /// </summary>
    /// <param name="readings">The optional <see cref="IEnumerable{ReadingsWebModel}"/> of <see cref="ReadingsWebModel"/></param>
    /// <param name="detailLevel">The <see cref="DetailLevel"/> enumerable.</param>
    /// <returns>The <see cref="IEnumerable{ReadingsWebModel}"/> of <see cref="ReadingsWebModel"/>.</returns>
    private static IEnumerable<ReadingsWebModel>? LimitDataCount(IEnumerable<ReadingsWebModel>? readings,
        DetailLevel detailLevel)
    {
        if (readings == null || !readings.Any())
        {
            return [];
        }

        int? limit = detailLevel switch
        {
            DetailLevel.Normal => 1000,
            DetailLevel.Less => 500,
            DetailLevel.Detailed => null
        };

        if (!limit.HasValue || readings.Count() <= limit.Value)
        {
            return readings;
        }

        int skip = readings.Count() / limit.Value;

        return GetNth(readings.ToList(), skip);
    }

    /// <summary>
    /// Gets every nth element
    /// </summary>
    /// <param name="readings">The <see cref="List{ReadingsWebModel}"/> of <see cref="ReadingsWebModel"/> </param>
    /// <param name="step">The step to limit data</param>
    /// <returns>The limited <see cref="IEnumerable{ReadingsWebModel}"/> of <see cref="ReadingsWebModel"/>.</returns>
    private static IEnumerable<ReadingsWebModel>? GetNth(List<ReadingsWebModel> readings, int step)
    {
        for (int i = 0; i < readings.Count; i += step)
        {
            yield return readings[i];
        }
    }
}