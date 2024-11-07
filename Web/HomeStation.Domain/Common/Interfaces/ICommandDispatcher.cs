using System.Windows.Input;

namespace HomeStation.Domain.Common.Interfaces;

/// <summary>
/// The command dispatcher interface
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches the specified command
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="clientId">The client Id.</param>
    /// <typeparam name="TCommand">The command type.</typeparam>
    Task Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken, string? clientId = null)
        where TCommand : class, ICommand;
}