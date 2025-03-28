namespace Loyalty.Infrastructure.External.Kafka.Consumers;

public record KafkaMessage
{
    public required string MessageKey { get; init; }
    public required IDictionary<string, string> Headers { get; init; }
    public required string Body { get; init; }
    public required string Type { get; set; }
}

