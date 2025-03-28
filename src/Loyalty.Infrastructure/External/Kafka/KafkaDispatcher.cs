using Confluent.Kafka;

namespace Loyalty.Infrastructure.External.Kafka;

public class KafkaDispatcher : IKafkaDispatcher
{
    private readonly IKafkaProducer _producer;

    public KafkaDispatcher(IKafkaProducer producer)
    {
        _producer = producer;
    }

    public Task<bool> ProduceAsync(string topic, Message<string, string> message, CancellationToken cancellationToken)
    {
        return _producer.ProduceAsync(topic, message, cancellationToken);
    }
}