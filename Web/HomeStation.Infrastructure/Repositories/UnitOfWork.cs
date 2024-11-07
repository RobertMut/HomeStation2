using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Virtual;
using HomeStation.Domain.Common.Entities;

namespace HomeStation.Infrastructure.Repositories;

/// <summary>
/// The unit of work class
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private IAirDbContext _dbContext;
    private Repository<Quality> _qualityRepository;
    private Repository<Climate> _climateRepository;
    private Repository<Device> _deviceRepository;

    private bool disposedValue = false;

    /// <summary>
    /// Initializes a new Unit of work
    /// </summary>
    /// <param name="dbContext">The <see cref="IAirDbContext"/></param>
    public UnitOfWork(IAirDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Air Quality repository getter
    /// </summary>
    public Repository<Quality> QualityRepository
    {
        get
        {
            if (_qualityRepository == null)
            {
                _qualityRepository = new Repository<Quality>(_dbContext);
            }
                
            return _qualityRepository;
        }
    }
        
    /// <summary>
    /// Climate repository getter
    /// </summary>
    public Repository<Climate> ClimateRepository
    {
        get
        {
            if(_climateRepository == null)
            {
                _climateRepository = new Repository<Climate>(_dbContext);
            }

            return _climateRepository;
        }
    }
    
    /// <summary>
    /// Device repository getter
    /// </summary>
    public Repository<Device> DeviceRepository
    {
        get
        {
            if(_deviceRepository == null)
            {
                _deviceRepository = new Repository<Device>(_dbContext);
            }

            return _deviceRepository;
        }
    }
    
    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="cancellationToken"></param>
    public virtual async Task Save(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes context
    /// </summary>
    /// <param name="disposing">Dispose Context</param>
    public virtual void Dispose(bool disposing)
    {
        if (!disposedValue && disposing)
        {
            _dbContext.DisposeAsync();
        }
        
        disposedValue = true;
    }

    /// <summary>
    /// Dispose method
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}