using System.Linq.Expressions;
using HomeStation.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace HomeStation.Application.Common.Virtual;

/// <summary>
/// The repository class
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class Repository<TEntity> where TEntity : class
{
    private IAirDbContext _dbContext;
    private DbSet<TEntity> _dbSet;
    
    /// <summary>
    /// Initializes new Repository
    /// </summary>
    /// <param name="dbContext">The <see cref="IAirDbContext"/></param>
    public Repository(IAirDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    /// <summary>
    /// Deletes from repository
    /// </summary>
    /// <param name="entity">Entity</param>
    public virtual void Delete(TEntity? entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Gets entities by filter
    /// </summary>
    /// <param name="predicate">Expression filter</param>
    /// <param name="includeProperties">Properties</param>
    /// <returns>IEnumerable of entities</returns>
    public virtual IQueryable<TEntity> Get(Func<IQueryable<TEntity>, IQueryable<TEntity>> func)
    {
        var query = _dbSet.AsQueryable();

        if (func != null)
        {
            query = func(query);
        }

        return query.AsNoTracking();
    }
    
    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="includeProperties">Properties</param>
    /// <returns>IEnumerable of entities</returns>
    public virtual IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = _dbSet.AsQueryable();

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return query;
    }
    
    /// <summary>
    /// Gets single entity object by filter
    /// </summary>
    /// <param name="filter">Expression filter</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Entity</returns>
    public virtual async Task<TEntity?> GetObjectBy(Func<TEntity?, bool> func,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        if (include != null)
        {
            query = include(query);
        }
        
        return query.FirstOrDefault(func);
    }

    /// <summary>
    /// Gets last single entity object by filter
    /// </summary>
    /// <param name="filter">Expression filter</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Entity</returns>
    public virtual async Task<TEntity?> GetLastBy(Func<TEntity?, bool> func,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        if (include != null)
        {
            query = include(query);
        }
        
        return query.LastOrDefault(func);
    }
    
    /// <summary>
    /// Inserts to dbset
    /// </summary>
    /// <param name="entity">Entity to be insterted</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Entity</returns>
    public virtual async Task<TEntity?> InsertAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
            
        return entry.Entity;
    }

    /// <summary>
    /// Updates entity
    /// </summary>
    /// <param name="entity">Entity to be updated</param>
    public virtual void UpdateAsync(TEntity? entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
    }
}