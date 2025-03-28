namespace Loyalty.Infrastructure.External.Kafka.Consumers.ConsumeResultProcessors;

public class MessageType
{
    public required string Name { get; init; }
    public required Type Type { get; set; }
}
