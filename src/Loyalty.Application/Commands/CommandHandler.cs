namespace Loyalty.Application.Commands;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public abstract Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}
