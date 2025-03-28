using Loyalty.Application.Commands;
using Loyalty.Application.Events;
using Loyalty.Domain.Core.Extensions;
using Loyalty.Infrastructure.External.Kafka.Consumers.BusMessages;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.PublicBusMessages;

/// <inheritdoc />
public class PublicBusMessageDispatcher : IPublicBusMessageDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Dictionary<Type, EventHandlerWrapper> _eventHandlers = [];
    private static readonly Dictionary<Type, CommandHandlerWrapper> _commandHandlers = [];

    public PublicBusMessageDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task DispatchAsync(IBusMessage busMessage, CancellationToken cancellationToken)
    {
        var messageType = busMessage.GetType();
        if (busMessage is IEvent @event)
        {
            var handler = _eventHandlers.GetOrSet(messageType, () => EventHandlerWrapper.Create(messageType));

            return handler.HandleAsync(_serviceProvider, @event, cancellationToken);
        }

        if (busMessage is ICommand command)
        {
            var handler = _commandHandlers.GetOrSet(messageType, () => CommandHandlerWrapper.Create(messageType));

            return handler.HandleAsync(_serviceProvider, command, cancellationToken);
        }

        throw new NotSupportedException($"Unsupported message type {busMessage.GetType()}");
    }
}
