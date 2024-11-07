using HomeStation.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HomeStation.Application.Common.Interfaces;

public interface IAirDbContext
{
    /// <summary>
    /// Climate readings
    /// </summary>
    public DbSet<Climate> Climate { get; set; }
    
    /// <summary>
    /// Air Quality readings
    /// </summary>
    public DbSet<Quality> AirQuality { get; set; }

    /// <summary>
    /// Devices
    /// </summary>
    public DbSet<Device> Devices { get; set; }
    
    DatabaseFacade Database { get; }

    ValueTask DisposeAsync();
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}