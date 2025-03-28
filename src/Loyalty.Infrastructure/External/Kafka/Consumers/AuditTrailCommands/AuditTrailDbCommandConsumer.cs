using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Loyalty.Infrastructure.External.Kafka.Consumers.ConsumeResultProcessors;
using Loyalty.Infrastructure.Settings;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.AuditTrailCommands;

/// <summary>
/// Consumes and processes events from the `oms-core-audit-trail-commands` Kafka topic.
/// </summary>
public class AuditTrailDbCommandConsumer : KafkaConsumer<IMessageConsumeResultProcessor, string, string>
{
    public AuditTrailDbCommandConsumer(
        IOptions<KafkaSettings> settings,
        ILogger<AuditTrailDbCommandConsumer> logger,
        IServiceScopeFactory serviceScopeFactory
    ) : base(
        settings: settings,
        topic: Topics.OmsCoreAuditTrailPrivateDbCommands,
        logger: logger,
        serviceScopeFactory: serviceScopeFactory
    )
    { }
}
