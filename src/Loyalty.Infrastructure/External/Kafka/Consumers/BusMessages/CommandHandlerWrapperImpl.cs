using Loyalty.Application.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.BusMessages;
public class CommandHandlerWrapperImpl<TCommand> : CommandHandlerWrapper
    where TCommand : ICommand
{
    public override async Task HandleAsync(IServiceProvider serviceProvider, ICommand command, CancellationToken cancellationToken)
    {
        var handlers = serviceProvider.GetServices<ICommandHandler<TCommand>>();
        if (!handlers.Any())
        {
            throw new ApplicationException($"No registered command handlers found for {command.GetType()}");
        }

        var exceptions = new List<Exception>();

        foreach (var handler in handlers)
        {
            try
            {
                await handler.ExecuteAsync((TCommand)command, cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException($"{exceptions.Count} command handler(s) completed with an exception", exceptions);
        }
    }
}
