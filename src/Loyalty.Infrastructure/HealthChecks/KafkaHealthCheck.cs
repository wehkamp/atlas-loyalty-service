using Loyalty.Infrastructure.External.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Loyalty.Infrastructure.HealthChecks;

public class KafkaHealthCheck : IHealthCheck
{
    private readonly IKafkaTopicHelper _kafkaTopicHelper;

    public KafkaHealthCheck(IKafkaTopicHelper kafkaTopicHelper)
    {
        _kafkaTopicHelper = kafkaTopicHelper;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            bool result = await _kafkaTopicHelper.KafkaTopicsExists();

            if (!result)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, description: "Kafka is down or not all topics exist.");
            }

            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, description: "Kafka is down or not all topics exist.", exception);
        }
    }
}
