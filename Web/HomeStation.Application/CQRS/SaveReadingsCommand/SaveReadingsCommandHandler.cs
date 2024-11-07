using System.Globalization;
using HomeStation.Application.Common.Exceptions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Domain.Common.Entities;
using HomeStation.Domain.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace HomeStation.Application.CQRS.SaveReadingsCommand;

/// <summary>
/// The class for handling incoming readigns
/// </summary>
public class SaveReadingsCommandHandler : ICommandHandler<SaveReadingsCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <inheritdoc cref="ICommandHandler{SaveReadingsCommand}"/>
    public SaveReadingsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Handles incoming readings
    /// </summary>
    /// <param name="command">The <see cref="SaveReadingsCommand"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <param name="clientId">The optional client id.</param>
    /// <exception cref="Exception">If readings are invalid or device not known.</exception>
    public async Task Handle(SaveReadingsCommand command, CancellationToken cancellationToken, string? clientId = null)
    {
        if (command.Pressure == 0)
        {
            throw new ReadingsException("Invalid readings. Pressure cannot be 0");
        }
        
        using (_unitOfWork)
        {
            Device? device = await _unitOfWork.DeviceRepository.GetObjectBy(x => x.Name == clientId, cancellationToken: cancellationToken);
            await CheckDevice(device, clientId, cancellationToken);

            (Climate, Quality) entities = ConstructEntities(command, device.Id);
            
            await _unitOfWork.ClimateRepository.InsertAsync(entities.Item1, cancellationToken);
            await _unitOfWork.QualityRepository.InsertAsync(entities.Item2, cancellationToken);

            await _unitOfWork.Save(cancellationToken);
        }
    }

    /// <summary>
    /// Checks if device sent data before
    /// </summary>
    /// <param name="device">The <see cref="Device"/></param>
    /// <param name="deviceId">The optional device id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <exception cref="Exception">If device is not known.</exception>
    private async Task CheckDevice(Device? device, string? deviceId, CancellationToken cancellationToken)
    {
        if (device == null)
        {
            device = new Device
            {
                Name = deviceId,
                IsKnown = false
            };
                
            await _unitOfWork.DeviceRepository.InsertAsync(device, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
        }

        if (!device.IsKnown)
        {
            throw new NotFoundException("Device is not known.");
        }
    }

    /// <summary>
    /// Constructs the required data to insert
    /// </summary>
    /// <param name="command">The <see cref="SaveReadingsCommand"/>.</param>
    /// <param name="deviceId">The device id.</param>
    /// <returns>The <see cref="Tuple{Climate,Quality}"/> of <see cref="Climate"/> and <see cref="Quality"/></returns>
    private static (Climate, Quality) ConstructEntities(SaveReadingsCommand command, int deviceId)
    {
        Reading reading = GetCurrentReading();
        
        var climate = new Climate
        {
            DeviceId = deviceId,
            Temperature = command.Temperature,
            Humidity = command.Humidity,
            Pressure = command.Pressure,
            Reading = reading
        };

        var quality = new Quality()
        {
            DeviceId = deviceId,
            Pm1_0 = command.Pm1_0,
            Pm2_5 = command.Pm2_5,
            Pm10 = command.Pm10,
            Reading = reading
        };

        return (climate, quality);  
    }

    /// <summary>
    /// Gets current date
    /// </summary>
    /// <returns>The current date in <see cref="Reading"/></returns>
    private static Reading GetCurrentReading() =>
        new()
        {
            Date = DateTimeOffset.Now,
        };
}