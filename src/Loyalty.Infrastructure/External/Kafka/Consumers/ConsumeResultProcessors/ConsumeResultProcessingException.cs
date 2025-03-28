using Loyalty.Application.Events;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.ConsumeResultProcessors;

public class ConsumeResultProcessingException : Exception
{
    public ConsumeResultProcessingException(string? messageName, IBusMessage? busMessage, Exception ex)
        : base($"Error processing event {messageName}: {ex.Message}", ex)
    {
        MessageName = messageName;
        BusMessage = busMessage;
    }

    public string? MessageName { get; }
    public IBusMessage? BusMessage { get; }
}
