namespace HomeStation.Domain.Common.Interfaces;

/// <summary>
/// The query dispatcher interface
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches specified query
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TQueryResult">The query result.</typeparam>
    /// <returns>The result of type <see cref="TQueryResult"/></returns>
    Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellationToken);
}