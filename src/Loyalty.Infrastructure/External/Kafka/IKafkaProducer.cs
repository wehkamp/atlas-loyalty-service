using Confluent.Kafka;

namespace Loyalty.Infrastructure.External.Kafka;

public interface IKafkaProducer
{
    /// <summary>
    /// Produces the given message to the given topic
    /// </summary>
    /// <param name="topic">Topic to which the message should be produced</param>
    /// <param name="message">Message to produce</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>True when the message was successfully produced, else false.</returns>
    Task<bool> ProduceAsync(string topic, Message<string, string> message, CancellationToken cancellationToken);
}
