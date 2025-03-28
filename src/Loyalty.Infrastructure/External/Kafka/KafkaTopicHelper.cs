using Confluent.Kafka;
using Loyalty.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Loyalty.Infrastructure.External.Kafka;

public class KafkaTopicHelper : IKafkaTopicHelper
{
    private readonly ILogger<KafkaTopicHelper> _logger;
    private readonly IOptions<KafkaSettings> _kafkaSettings;

    public KafkaTopicHelper(
        ILogger<KafkaTopicHelper> logger,
        IOptions<KafkaSettings> kafkaSettings
    )
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings;
    }

    public async Task<bool> KafkaTopicsExists()
    {
        using var client = CreateClient();

        _logger.LogTrace(message: "Checking if all topics exist.");

        // Get all configured topics needed for consumption
        var topics = new List<string>();
        foreach (string configuredTopic in _kafkaSettings.Value.ConfiguredTopics)
        {
            var topic = Topics.OmsCoreAuditTrailTopics.SingleOrDefault(topic => topic.Equals(configuredTopic, StringComparison.InvariantCultureIgnoreCase));

            if (topic == null)
            {
                _logger.LogError(message: $"Configured topic {configuredTopic} does not exist in the list of known topics.");
                
#if DEBUG
                if (Debugger.IsAttached)
                {
                    // Trigger a breakpoint, so you know not all topics exists on your machine
                    Debugger.Break();
                }
#endif
                return false;
            }

            topics.Add(topic);
        }

        // Check if they exists
        await client.DescribeTopicsAsync(TopicCollection.OfTopicNames(topics));
        _logger.LogTrace(message: "All topics exist.");

        return true;
    }

    private IAdminClient CreateClient()
    {
        var config = new AdminClientConfig
        {
            BootstrapServers = _kafkaSettings.Value.BootstrapServers,
            SecurityProtocol = _kafkaSettings.Value.UseSsl ? SecurityProtocol.Ssl : SecurityProtocol.Plaintext,
        };
        return new AdminClientBuilder(config).Build();
    }
}
