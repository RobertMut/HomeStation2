
namespace HomeStation.Domain.Common.Interfaces;

/// <summary>
/// The command handler interface
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
{ 
    /// <summary>
    /// Handles command
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <param name="clientId">The client id.</param>
    Task Handle(TCommand command, CancellationToken cancellationToken, string? clientId = null);
}