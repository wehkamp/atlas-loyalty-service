using Confluent.Kafka;
using Loyalty.Application.Commands;
using Loyalty.Application.Events;
using Loyalty.Infrastructure.External.Kafka.Consumers.PublicBusMessages;
using Microsoft.Extensions.Logging;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.ConsumeResultProcessors;

public class MessageConsumeResultProcessor : IMessageConsumeResultProcessor
{
    private readonly IApplicationCatalog _applicationCatalog;
    private readonly IPublicBusMessageDispatcher _busMessageDispatcher;
    private readonly ILogger<MessageConsumeResultProcessor> _logger;

    public MessageConsumeResultProcessor(
        IApplicationCatalog applicationCatalog,
        IPublicBusMessageDispatcher busMessageDispatcher,
        ILogger<MessageConsumeResultProcessor> logger
    )
    {
        _logger = logger;
        _applicationCatalog = applicationCatalog;
        _busMessageDispatcher = busMessageDispatcher;
    }

    public async Task ProcessAsync(ConsumeResult<string, string> result, CancellationToken cancellationToken)
    {
        var message = result.Message;
        MessageType? messageType = null;
        IBusMessage? busMessage = null;
        try
        {
            string eventSerialized = message.Value;

            if (message.Headers is null)
            {
                throw new NotSupportedException("Processing a Kafka message without headers is not allowed");
            }

            messageType = KafkaConsumerHelper.GetBusMessageType(_applicationCatalog, message.Headers);

            if (typeof(IEvent).IsAssignableFrom(messageType.Type))
            {
                busMessage = KafkaEventSerializer.DeserializeEvent(eventSerialized, messageType.Type)!;
            }

            if (typeof(ICommand).IsAssignableFrom(messageType.Type))
            {
                busMessage = KafkaEventSerializer.DeserializeCommand(eventSerialized, messageType.Type)!;
            }

            if (busMessage == null)
            {
                throw new InvalidOperationException($"Unsupported message type {messageType?.Name}");
            }

            await _busMessageDispatcher.DispatchAsync(busMessage, CancellationToken.None);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            throw new ConsumeResultProcessingException(messageType?.Name, busMessage, ex);
        }
    }
}
