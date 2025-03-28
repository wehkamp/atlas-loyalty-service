using Loyalty.Application.Models;

namespace Loyalty.Application.Commands;
public abstract class InMemoryCommandHandler<TCommand> : IInMemoryCommandHandler<TCommand>
    where TCommand : IInMemoryCommand
{
    public abstract Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);

    async Task<Nothing> IInMemoryCommandHandler<TCommand, Nothing>.ExecuteAsync(TCommand command, CancellationToken cancellationToken)
    {
        await ExecuteAsync(command, cancellationToken);
        return new Nothing();
    }
}
