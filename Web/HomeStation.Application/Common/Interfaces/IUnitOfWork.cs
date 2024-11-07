using HomeStation.Application.Common.Virtual;
using HomeStation.Domain.Common.Entities;

namespace HomeStation.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Air Quality repository getter
    /// </summary>
    Repository<Quality> QualityRepository { get; }

    /// <summary>
    /// Climate repository getter
    /// </summary>
    Repository<Climate> ClimateRepository { get; }

    /// <summary>
    /// Device repository getter
    /// </summary>
    Repository<Device> DeviceRepository { get; }

    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task Save(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disposes context
    /// </summary>
    /// <param name="disposing">Dispose Context</param>
    void Dispose(bool disposing);
}