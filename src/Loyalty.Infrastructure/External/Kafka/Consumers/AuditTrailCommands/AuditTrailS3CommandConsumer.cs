using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Loyalty.Infrastructure.External.Kafka.Consumers.ConsumeResultProcessors;
using Loyalty.Infrastructure.Settings;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.AuditTrailCommands;

/// <summary>
/// Consumes and processes events from the `oms-core-audit-trail-commands` Kafka topic.
/// </summary>
public class AuditTrailS3CommandConsumer : KafkaConsumer<IMessageConsumeResultProcessor, string, string>
{
    public AuditTrailS3CommandConsumer(
        IOptions<KafkaSettings> settings,
        ILogger<AuditTrailS3CommandConsumer> logger,
        IServiceScopeFactory serviceScopeFactory
    ) : base(
        settings: settings,
        topic: Topics.OmsCoreAuditTrailPrivateS3Commands,
        logger: logger,
        serviceScopeFactory: serviceScopeFactory
    )
    { }
}
