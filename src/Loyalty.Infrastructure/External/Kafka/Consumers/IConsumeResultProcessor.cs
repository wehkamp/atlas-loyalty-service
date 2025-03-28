using Confluent.Kafka;

namespace Loyalty.Infrastructure.External.Kafka.Consumers;

public interface IConsumeResultProcessor<TKey, TMessage>
{
    Task ProcessAsync(ConsumeResult<TKey, TMessage> result, CancellationToken cancellationToken);
}
