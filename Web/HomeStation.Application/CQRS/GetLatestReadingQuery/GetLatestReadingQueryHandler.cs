using HomeStation.Application.Common.Exceptions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Domain.Common.Entities;
using HomeStation.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeStation.Application.CQRS.GetLatestReadingQuery;

/// <summary>
/// The class to get latest reading
/// </summary>
public class GetLatestReadingQueryHandler : IQueryHandler<GetLatestReadingQuery, ReadingsWebModel>
{    
    private readonly IUnitOfWork _unitOfWork;

    /// <inheritdoc cref="IQueryHandler{GetLatestReadingQuery,ReadingsWebModel}"/>
    public GetLatestReadingQueryHandler(IUnitOfWork unitOfWork) 
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Handles get latest reading query
    /// </summary>
    /// <param name="query">The <see cref="GetLatestReadingQuery"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ReadingsWebModel"/></returns>
    /// <exception cref="Exception">If there is no readings</exception>
    public async Task<ReadingsWebModel> Handle(GetLatestReadingQuery query, CancellationToken cancellationToken)
    {
        Climate? climate = await _unitOfWork.ClimateRepository.GetLastBy(x => x.DeviceId == query.DeviceId, cancellationToken: cancellationToken);
        Quality? quality = await _unitOfWork.QualityRepository.GetLastBy(x => x.DeviceId == query.DeviceId, cancellationToken: cancellationToken);
        
        if (climate == null || quality == null)
        {
            throw new NotFoundException("No readings found");
        }

        return new ReadingsWebModel()
        {
            DeviceId = query.DeviceId,
            Humidity = climate.Humidity,
            Pressure = climate.Pressure,
            Temperature = climate.Temperature,
            Pm1_0 = quality.Pm1_0,
            Pm2_5 = quality.Pm2_5,
            Pm10 = quality.Pm10,
            ReadDate = quality.Reading.Date
        };
    }
}