using Loyalty.Application.Models;

namespace Loyalty.Application.Commands;

public interface IInMemoryCommandHandler<TCommand, TResult>
    where TCommand : IInMemoryCommand<TResult>
{
    Task<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}

public interface IInMemoryCommandHandler<TCommand> : IInMemoryCommandHandler<TCommand, Nothing>
    where TCommand : IInMemoryCommand
{ }
