using Loyalty.Application.Commands;

namespace Loyalty.Application.Services;

public interface IAuditTrailDbCommandPublisher : IScopedService
{
    Task PublishAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand;

    public async Task PublishAsync<TCommand>(IEnumerable<TCommand> commands, CancellationToken cancellationToken) where TCommand : ICommand
    {
        foreach (var command in commands)
        {
            await PublishAsync(command: command, cancellationToken: cancellationToken);
        }
    }
}
