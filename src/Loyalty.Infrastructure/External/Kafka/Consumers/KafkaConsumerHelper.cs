using Confluent.Kafka;
using Loyalty.Infrastructure.External.Kafka.Consumers.ConsumeResultProcessors;
using System.Text;

namespace Loyalty.Infrastructure.External.Kafka.Consumers;

public static class KafkaConsumerHelper
{
    public const string EventNameHeader = "event-name";
    public const string CommandNameHeader = "command-name";

    public const string LabelHeader = "label";
    public const string AtlasLabelHeader = "atlas-label";

    public const string OriginalTopicHeader = "original-topic";
    public const string RetryAttemptHeader = "retry-attempt";
    public const string RetryTimestampHeader = "retry-timestamp";
    public const string TypeHeader = "type";
    public const string CorrelationIdHeader = "correlation-id";

    public const string DefaultFallback = "unknown";

    public static MessageType GetBusMessageType(IApplicationCatalog applicationCatalog, Headers headers)
    {
        string? messageName = null;
        Type? applicationCatalogType = null;

        // OMS events (public & private)
        if (headers.TryGetLastBytes(EventNameHeader, out var eventNameEncoded))
        {
            messageName = Encoding.UTF8.GetString(eventNameEncoded);
            applicationCatalogType = applicationCatalog.GetEventTypeByName(messageName);
        }
        // OMS commands (public & private)
        if (headers.TryGetLastBytes(CommandNameHeader, out var commandNameEncoded))
        {
            messageName = Encoding.UTF8.GetString(commandNameEncoded);
            applicationCatalogType = applicationCatalog.GetCommandTypeByName(messageName);
        }
        // Final check that every thing is alright before we dispatch
        if (string.IsNullOrWhiteSpace(messageName) ||
            applicationCatalogType == null)
        {
            throw new NotSupportedException($"No kafka headers found for a message with expected header with message {messageName}");
        }

        return new MessageType
        {
            Name = messageName,
            Type = applicationCatalogType,
        };
    }

    // Support for decoration of the metrics
    public static string GetName(Message<string, string> message)
    {
        if (message.Headers == null)
        {
            return DefaultFallback;
        }

        // OMS events (public & private)
        if (message.Headers.TryGetLastBytes(EventNameHeader, out var eventNameEncoded))
        {
            return Encoding.UTF8.GetString(eventNameEncoded);
        }
        // OMS commands (public & private)
        if (message.Headers.TryGetLastBytes(CommandNameHeader, out var commandNameEncoded))
        {
            return Encoding.UTF8.GetString(commandNameEncoded);
        }

        return DefaultFallback;
    }

    public static string? GetLabel(Message<string, string> message)
    {
        if (message.Headers is null)
        {
            return null;
        }

        if (message.Headers.TryGetLastBytes(LabelHeader, out var labelEncoded))
        {
            return Encoding.UTF8.GetString(labelEncoded);
        }

        return null;
    }

    public static string KafkaToString(byte[] valueBytes)
        => Encoding.UTF8.GetString(valueBytes);

    public static byte[] StringToKafka(string value)
        => Encoding.UTF8.GetBytes(value);
}

