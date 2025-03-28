using Loyalty.Application.Commands;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.BusMessages;

public abstract class CommandHandlerWrapper
{
    public abstract Task HandleAsync(IServiceProvider serviceProvider, ICommand command, CancellationToken cancellationToken);

    public static CommandHandlerWrapper Create(Type commandType)
    {
        return (CommandHandlerWrapper)(Activator.CreateInstance(typeof(CommandHandlerWrapperImpl<>).MakeGenericType(commandType))
                                       ?? throw new InvalidOperationException($"Could not create wrapper for command type {commandType}"));
    }
}
