using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Loyalty.Infrastructure.External.Kafka.Consumers.ConsumeResultProcessors;
using Loyalty.Infrastructure.Settings;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.AuditTrailCommands;

/// <summary>
/// Consumes and processes events from the `oms-core-audit-trail-commands` Kafka topic.
/// </summary>
public class AuditTrailCommandConsumer : KafkaConsumer<IMessageConsumeResultProcessor, string, string>
{
    public AuditTrailCommandConsumer(
        IOptions<KafkaSettings> settings,
        ILogger<AuditTrailCommandConsumer> logger,
        IServiceScopeFactory serviceScopeFactory
    ) : base(
        settings: settings,
        topic: Topics.OmsCoreAuditTrailCommandsPublic,
        logger: logger,
        serviceScopeFactory: serviceScopeFactory
    )
    { }
}
