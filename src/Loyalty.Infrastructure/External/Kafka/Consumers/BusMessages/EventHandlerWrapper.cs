using Loyalty.Application.Events;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.BusMessages;

public abstract class EventHandlerWrapper
{
    public abstract Task HandleAsync(IServiceProvider serviceProvider, IEvent @event, CancellationToken cancellationToken);

    public static EventHandlerWrapper Create(Type eventType)
    {
        return (EventHandlerWrapper)(Activator.CreateInstance(typeof(EventHandlerWrapperImpl<>).MakeGenericType(eventType))
                                     ?? throw new InvalidOperationException($"Could not create wrapper for event type {eventType}"));
    }
}
