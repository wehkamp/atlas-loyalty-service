namespace Loyalty.Infrastructure.External.Kafka;
public interface IKafkaTopicHelper
{
    Task<bool> KafkaTopicsExists();
}
