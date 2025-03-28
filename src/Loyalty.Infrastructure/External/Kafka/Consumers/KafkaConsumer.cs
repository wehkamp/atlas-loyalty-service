using Confluent.Kafka;
using Loyalty.Application.Repositories;
using Loyalty.Domain.Core;
using Loyalty.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Loyalty.Infrastructure.External.Kafka.Exceptions;
using Loyalty.Infrastructure.Logging;
using System.Text.Json;
using InvalidOperationException = System.InvalidOperationException;

namespace Loyalty.Infrastructure.External.Kafka.Consumers;

public abstract class KafkaConsumer<TIConsumeResultProcessor, TKey, TMessage> : BackgroundService
    where TIConsumeResultProcessor : IConsumeResultProcessor<TKey, TMessage>
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _topic;
    private IConsumer<TKey, TMessage>? _consumer;

    protected KafkaConsumer(
        IOptions<KafkaSettings> settings,
        string topic,
        ILogger logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        Settings = settings.Value;
        _topic = topic;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected KafkaSettings Settings { get; }

    protected virtual ConsumerBuilder<TKey, TMessage> CreateConsumer(ConsumerConfig config)
        => new(config);

    private IDictionary<string, string> ToLogState(Message<TKey, TMessage> message)
    {
        var result = new Dictionary<string, string>();
        try
        {
            result.Add("kafka-message", JsonSerializer.Serialize(message));
        }
        catch (Exception ex)
        {
            result.Add("kafka-message", $"Error serializing the message: {ex.Message}");
        }

        return result;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (Settings.EnableConsumers)
        {
            await Task.Yield();

            try
            {
                // Start the listeners
                Start();

                // Start consuming messages
                await ConsumeAsync(cancellationToken);
            }
            finally
            {
                Stop();
            }
        }
    }

    protected virtual void SetLabelHeader(IServiceScope scope, KafkaMessage kafkaMessage, CancellationToken cancellationToken)
    {
        // Process the label out of the header
        var headers = kafkaMessage.Headers;
        if (headers is null)
        {
            throw new InvalidOperationException($"No kafka headers are set for message with key: {kafkaMessage.MessageKey}");
        }

        string? label = null;
        // Get the label header with the new atlas-label we consistently use now
        if (headers.TryGetValue(KafkaConsumerHelper.AtlasLabelHeader, out var atlasLabelHeader))
        {
            label = atlasLabelHeader;
        }
        // Get the label header (before atlas-label)
        else if (headers.TryGetValue(KafkaConsumerHelper.LabelHeader, out var labelHeader))
        {
            label = labelHeader;
        }
        // It's not an OMS event.
        else
        {
            throw new InvalidOperationException($"AtlasLabel header does not exists in this message {kafkaMessage.MessageKey}");
        }

        // Get the label out of the database, so we can set the label configuration, TODO(NM): Fix this
        var labelRepository = scope.ServiceProvider.GetRequiredService<ILabelRepository>();
        var labelConfiguration = labelRepository.GetLabel(label!);

        if (labelConfiguration == null)
        {
            throw new LabelNotFoundException(label);
        }

        // Get the Fraud Scoped context
        scope.UseLabel(labelConfiguration!.Value);
    }
    
    private void SetCorrelationId(IServiceScope scope, KafkaMessage kafkaMessage)
    {
        // Process the label out of the header
        if (kafkaMessage.Headers is null)
        {
            throw new InvalidOperationException($"No kafka headers are set for message with key: {kafkaMessage.MessageKey}");
        }

        var scopedContext = scope.ServiceProvider.GetRequiredService<IOmsAuditTrailScopedContext>();
        if (!kafkaMessage.Headers.TryGetValue(KafkaConsumerHelper.CorrelationIdHeader, out var correlationIdValue))
        {
            _logger.LogWarning("Consuming an event without correlation id. This should only happen during the deployment and right after it for message {kafkaMessageKey}", kafkaMessage.MessageKey);
            scopedContext.CorrelationId = Guid.NewGuid();
        }
        else
        {
            if (Guid.TryParse(correlationIdValue, out var correlationId))
            {
                // Because we also have external teams connecting with us, we need to check if the correlation id is empty. 
                if (correlationId != Guid.Empty)
                {
                    // We received a valid correlation id in the kafka message
                    scopedContext.CorrelationId = correlationId;
                }
                else
                {
                    // The current correlation id is not a valid one. Generate a new one.
                    var fallbackCorrelationId = Guid.NewGuid();
                    _logger.LogWarning("Consuming an event with an empty correlation id for kafka message {kafkaMessageKey} falling back on correlation id: {fallbackCorrelationId}", kafkaMessage.MessageKey, fallbackCorrelationId);
                    scopedContext.CorrelationId = fallbackCorrelationId;
                }
            }
            else
            {
                // The current correlation id is not a valid one. Generate a new one.
                var fallbackCorrelationId = Guid.NewGuid();
                _logger.LogWarning("Consuming an event with a correlationId that can't be parsed to a Guid for kafka message {kafkaMessageKey} with value {correlationIdValue} falling back on correlation id: {correlationIdValue}", kafkaMessage.MessageKey, correlationIdValue, fallbackCorrelationId);
                scopedContext.CorrelationId = fallbackCorrelationId;
            }
        }
    }

    private void SetDefaultScope(IServiceScope scope, KafkaMessage kafkaMessage, CancellationToken cancellationToken)
    {
        SetLabelHeader(scope: scope, kafkaMessage: kafkaMessage, cancellationToken: cancellationToken);
        SetCorrelationId(scope: scope, kafkaMessage: kafkaMessage);
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var stoppingTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            ConsumeResult<TKey, TMessage>? consumeResult = null;
            try
            {
                consumeResult = _consumer!.Consume(TimeSpan.FromSeconds(1));

                if (consumeResult is not null)
                {
                    var kafkaMessage = new KafkaMessage
                    {
                        MessageKey = consumeResult.Message.Key?.ToString() ?? string.Empty,
                        Headers = new Dictionary<string, string>(),
                        Body = consumeResult.Message.Value?.ToString() ?? string.Empty,
                        Type = typeof(TMessage).ToString(),
                    };

                    // Due to some duplicate keys sometimes we need to look if the key is already in the dictionary
                    foreach (IHeader header in consumeResult.Message.Headers.ToList())
                    {
                        if (kafkaMessage.Headers.ContainsKey(header.Key))
                        {
                            _logger.LogWarning("Duplicate header key {key} in message with key {messageKey}", header.Key, kafkaMessage.MessageKey);
                            continue;
                        }

                        kafkaMessage.Headers.Add(header.Key, KafkaConsumerHelper.KafkaToString(header.GetValueBytes()));

                        // Set the type of the message if we have it in the header. Else leave it on the BackgroundWorker injected type
                        if (header.Key == KafkaConsumerHelper.TypeHeader)
                        {
                            kafkaMessage.Type = KafkaConsumerHelper.KafkaToString(header.GetValueBytes());
                        }
                    }

                    await using var scope = _serviceScopeFactory.CreateAsyncScope();

                    // Now we can get all providers scoped
                    // var metrics = scope.ServiceProvider.GetRequiredService<IMetrics>();
                    // var counter = metrics.CreateCounter("oms_kafka_consumed_counter", "Number of event messages consumed from Kafka", "topic");

                    // Because it's a BackgroundService, we need to handle the stopping token when the SIGTERM is fired by the k8s cluster
                    // when the StoppingToken is triggered, we need to cancel the newly created cancellationTokenSource after 10 seconds
                    // this way we have 10 seconds to finish the processing of the message. The maximum time we have before the k8s cluster removes
                    // us is around 120 second. So we have some time to tweak if processes are taking to longer then 10 seconds. (but that is already a long time)
                    var cancellationTokenSource = new CancellationTokenSource();
                    // Link the CancellationToken to the StoppingToken with the timeout
                    stoppingTokenSource.Token.Register(() =>
                    {
                        _logger.LogWarning("Stopping token triggered. Cancel consuming token after 10 seconds for topic: {topic}", _topic);
                        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
                    });

                    // TODO(RW) Remove this after correct consuming
                    try
                    {
                        SetDefaultScope(scope: scope, kafkaMessage: kafkaMessage, cancellationToken: cancellationTokenSource.Token);
                    }
                    catch (InvalidOperationException)
                    {
                        _logger.LogWarning("Skipping message due to no headers for topic: {topic} of message: {kafkaMessageId}", _topic, kafkaMessage.MessageKey);
                        _consumer.Commit(consumeResult);
                        continue;
                    }
                    catch (LabelNotFoundException)
                    {
                        _logger.LogWarning("Skipping message due to no headers for topic: {topic} of message: {kafkaMessageId}", _topic, kafkaMessage.MessageKey);
                        _consumer.Commit(consumeResult);
                        continue;
                    }

                    var omsAuditTrailScopedContext = scope.ServiceProvider.GetRequiredService<IOmsAuditTrailScopedContext>();

                    IDictionary<string, object> loggingPropertiesDictionary = new Dictionary<string, object>
                    {
                        { "kafka-topic", consumeResult.Topic },
                        { "kafka-message-id", consumeResult.Message.Key?.ToString() ?? string.Empty },
                        { "kafka-message-type", kafkaMessage.Type },
                    };
                    using var loggerScope = _logger.BeginOmsCoreLogScope(kafkaMessage, "kafkaMessage", omsAuditTrailScopedContext, loggingProperties: loggingPropertiesDictionary);
                    
                    Exception? exception = null;

                    try
                    {
                        // Get the result processor in our scoped context and the decorators
                        // should be scoped now as well
                        _logger.LogDebug("Kafka consumer start consuming");
                        var consumeResultProcessor = scope.ServiceProvider.GetRequiredService<TIConsumeResultProcessor>();
                        await consumeResultProcessor.ProcessAsync(consumeResult, cancellationTokenSource.Token);
                        _logger.LogDebug("Kafka consumer finished processing");

                        _consumer.Commit(consumeResult);
                    }
                    catch (KafkaException kafkaException)
                    {
                        exception = kafkaException;
                        _logger.LogWarning(kafkaException, "Commit on {topic} partition {partition} offset {offset} key {key} failed", consumeResult.Topic, consumeResult.Partition, consumeResult.Offset, consumeResult.Message.Key);
                    }
                    catch (Exception innerException) when (!Topics.OmsCoreAuditTrailTopics.Contains(consumeResult.Topic, StringComparer.InvariantCultureIgnoreCase))
                    {
                        _logger.LogError(innerException, "OMS consume error for a topic that is not managed by OMS. Can't consume message with id {messageKey}", kafkaMessage.MessageKey);
                        // Don't throw an exception here. Just don't commit the message and try to consume the message again in the next run.
                    }
                }
            }
            catch (ConsumeException consumeException)
            {
                _logger.LogError(consumeException, consumeException.Message);
                _logger.LogInformation("Wait 60 seconds before resume consuming messages");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingTokenSource.Token);
            }
            catch (ObjectDisposedException) { }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred while processing KafkaConsumer {ConsumeResultProcessorType} of topic {topic}", typeof(TIConsumeResultProcessor).FullName, _topic);

                if (consumeResult is null)
                {
                    _logger.LogError(ex, "Consumer exception occurred");
                }
                else
                {
                    using (_logger.BeginScope(ToLogState(consumeResult.Message)))
                    {
                        _logger.LogError(ex, "Consumer error while processing {topic} partition {partition} offset {offset} key {key}", consumeResult.Topic, consumeResult.Partition, consumeResult.Offset, consumeResult.Message.Key);
                    }
                }

                throw;
            }
        }
    }

    protected virtual int MaxPollIntervalSeconds => 300;

    private void Start()
    {
        _logger.LogInformation("Starting consumer: {topic} with {brokers}", _topic, Settings.BootstrapServers);

        var config = new ConsumerConfig
        {
            BootstrapServers = Settings.BootstrapServers,
            GroupId = Settings.GroupName,
            EnableAutoCommit = false,
            QueuedMaxMessagesKbytes = 5120,
            QueuedMinMessages = 100,
            StatisticsIntervalMs = 1000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = false,
            SecurityProtocol = Settings.UseSsl ? SecurityProtocol.Ssl : SecurityProtocol.Plaintext,
            MaxPollIntervalMs = MaxPollIntervalSeconds * 1000,
        };

        var consumerName = GetType().Name;

        var consumerBuilder = CreateConsumer(config)
            .SetErrorHandler(
                (consumer, e) => _logger.LogWarning("{Topic} error consuming message of {Consumer}: [{Code}]: {Reason}", _topic, consumerName, e.Code, e.Reason)
            )
            .SetPartitionsAssignedHandler(
                (consumer, list) => _logger.LogInformation("{Topic} partitions assigned to {Consumer}: {Partitions}", _topic, consumerName, list.Select(p => p.Partition.Value))
            )
            .SetPartitionsLostHandler(
                (consumer, list) => _logger.LogInformation("{Topic} partitions lost by {Consumer}: {Partitions}", _topic, consumerName, list.Select(p => p.Partition.Value))
            )
            .SetPartitionsRevokedHandler(
                (consumer, list) => _logger.LogInformation("{Topic} partitions revoked from {Consumer}: {Partitions}", _topic, consumerName, list.Select(p => p.Partition.Value))
            );

        _consumer = consumerBuilder.Build();

        _consumer.Subscribe(_topic);
    }

    private void Stop()
    {
        _logger.LogInformation("Stopping consumer: {topic}", _topic);

        _consumer?.Close();
        _consumer?.Dispose();
    }
}
