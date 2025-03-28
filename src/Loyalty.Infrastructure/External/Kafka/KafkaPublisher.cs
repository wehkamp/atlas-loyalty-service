using Confluent.Kafka;
using Loyalty.Application;
using Loyalty.Application.Commands;
using Loyalty.Application.Events;
using Loyalty.Infrastructure.External.Kafka.Consumers;
using Microsoft.Extensions.Logging;
using Loyalty.Domain.Core;

namespace Loyalty.Infrastructure.External.Kafka;

public abstract class KafkaPublisher<TMessage>
    where TMessage : IBusMessage
{
    private readonly IKafkaDispatcher _producer;
    private readonly IApplicationCatalog _applicationCatalog;
    private readonly IOmsAuditTrailScopedContext _omsAuditTrailScopedContext;

    private readonly ILogger<KafkaPublisher<TMessage>> _baseLogger;
    // private readonly ICounter _eventCounter;
    // private readonly ICounter _commandCounter;

    protected KafkaPublisher(
        // IMetrics metrics,
        IKafkaDispatcher producer,
        IApplicationCatalog applicationCatalog,
        IOmsAuditTrailScopedContext omsAuditTrailScopedContext,
        ILogger<KafkaPublisher<TMessage>> baseLogger
    )
    {
        _producer = producer;
        _applicationCatalog = applicationCatalog;
        _omsAuditTrailScopedContext = omsAuditTrailScopedContext;
        _baseLogger = baseLogger;
        // _eventCounter = metrics.CreateCounter("oms_kafka_event_produced_count", "Number of event messages produced to Kafka", "topic", "event");
        // _commandCounter = metrics.CreateCounter("oms_kafka_command_produced_count", "Number of command messages produced to Kafka", "topic", "command");
    }

    private async Task PublishEventAsync(string topic, IEvent @event, CancellationToken cancellationToken)
    {
        var eventName = _applicationCatalog.GetEventName(@event.GetType());
        try
        {
            var message = new Message<string, string>
            {
                Key = @event.Id,
                Value = KafkaEventSerializer.SerializeEvent(@event),
                Headers = new Headers
                {
                    { KafkaConsumerHelper.EventNameHeader, KafkaConsumerHelper.StringToKafka(eventName) },
                    { KafkaConsumerHelper.TypeHeader, KafkaConsumerHelper.StringToKafka(@event.GetType().ToString()) },
                    { KafkaConsumerHelper.AtlasLabelHeader, KafkaConsumerHelper.StringToKafka(_omsAuditTrailScopedContext.AtlasLabel.Value) },
                    { KafkaConsumerHelper.LabelHeader, KafkaConsumerHelper.StringToKafka(_omsAuditTrailScopedContext.AtlasLabel.Value) },
                    { KafkaConsumerHelper.CorrelationIdHeader, KafkaConsumerHelper.StringToKafka(_omsAuditTrailScopedContext.CorrelationId.ToString()) },
                },
            };

            await _producer.ProduceAsync(topic, message, cancellationToken);

            // _eventCounter.Inc(topic, eventName);
        }
        catch (Exception ex)
        {
            _baseLogger.LogError("{err}", ex);
            // _eventCounter.Inc(ex, topic, eventName);
            throw;
        }
    }

    private async Task PublishCommandAsync(string topic, ICommand command, CancellationToken cancellationToken)
    {
        var commandName = command.GetCommandName();
        try
        {
            var message = new Message<string, string>
            {
                Key = command.Id,
                Value = KafkaEventSerializer.SerializeCommand(command),
                Headers = new Headers
                {
                    { KafkaConsumerHelper.CommandNameHeader, KafkaConsumerHelper.StringToKafka(commandName) },
                    { KafkaConsumerHelper.TypeHeader, KafkaConsumerHelper.StringToKafka(command.GetType().ToString()) },
                    { KafkaConsumerHelper.AtlasLabelHeader, KafkaConsumerHelper.StringToKafka(_omsAuditTrailScopedContext.AtlasLabel.Value) },
                    { KafkaConsumerHelper.LabelHeader, KafkaConsumerHelper.StringToKafka(_omsAuditTrailScopedContext.AtlasLabel.Value) },
                    { KafkaConsumerHelper.CorrelationIdHeader, KafkaConsumerHelper.StringToKafka(_omsAuditTrailScopedContext.CorrelationId.ToString()) },
                },
            };

            await _producer.ProduceAsync(topic, message, cancellationToken);

            // _commandCounter.Inc(topic, commandName);
        }
        catch (Exception ex)
        {
            _baseLogger.LogError("{exception}", ex);
            // _commandCounter.Inc(ex, topic, commandName);
            throw;
        }
    }

    protected async Task PublishAsync(string topic, TMessage busMessage, CancellationToken cancellationToken)
    {
        if (busMessage is IEvent @event)
        {
            await PublishEventAsync(topic: topic, @event: @event, cancellationToken: cancellationToken);
            return;
        }

        if (busMessage is ICommand command)
        {
            await PublishCommandAsync(topic: topic, command: command, cancellationToken: cancellationToken);
            return;
        }

        throw new InvalidOperationException("A kafka message should always be of the type IEvent or ICommand");
    }
}
