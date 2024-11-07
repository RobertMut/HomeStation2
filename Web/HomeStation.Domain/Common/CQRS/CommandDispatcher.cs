using HomeStation.Domain.Common.Interfaces;

namespace HomeStation.Domain.Common.CQRS;

/// <summary>
/// The command dispatcher class
/// </summary>
public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// Initializes new instance
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/></param>
    public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <summary>
    /// Dispatches the specified command
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="clientId">The client Id.</param>
    /// <typeparam name="TCommand">The command type.</typeparam>
    public async Task Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken, string? clientId = null)
        where TCommand : class, ICommand
    {
        ICommandHandler<TCommand> handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        await handler.Handle(command, cancellationToken, clientId);
    }
}