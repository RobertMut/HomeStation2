namespace HomeStation.Domain.Common.Interfaces;

/// <summary>
/// The query handler interface
/// </summary>
/// <typeparam name="TQuery">The query.</typeparam>
/// <typeparam name="TQueryResult">The returned values.</typeparam>
public interface IQueryHandler<in TQuery, TQueryResult>
{
    /// <summary>
    /// Handles query
    /// </summary>
    /// <param name="query">The query</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>The result of type </returns>
    Task<TQueryResult> Handle(TQuery query, CancellationToken cancellationToken);
}