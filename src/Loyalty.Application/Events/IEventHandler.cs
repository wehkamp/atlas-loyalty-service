namespace Loyalty.Application.Events;

public interface IEventHandler<TEvent>
    where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);

    public string Name => GetType().Name;
}

