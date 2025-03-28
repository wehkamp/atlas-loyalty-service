using Loyalty.Application.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.BusMessages;

public class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper
    where TEvent : IEvent
{
    public override async Task HandleAsync(IServiceProvider serviceProvider, IEvent @event, CancellationToken cancellationToken)
    {
        var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
        if (!handlers.Any())
        {
            throw new ApplicationException($"No registered event handlers found for {@event.GetType()}");
        }

        var exceptions = new List<Exception>();

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync((TEvent)@event, cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException($"{exceptions.Count} event handler(s) completed with an exception", exceptions);
        }
    }
}
