using HomeStation.Domain.Common.Interfaces;

/// <summary>
/// The query dispatcher class
/// </summary>
public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes new instance
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/></param>
    public QueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <summary>
    /// Dispatches the specified query
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TQueryResult">The query result type.</typeparam>
    /// <returns>The query result.</returns>
    public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellationToken)
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();
        
        return handler.Handle(query, cancellationToken);
    }
}