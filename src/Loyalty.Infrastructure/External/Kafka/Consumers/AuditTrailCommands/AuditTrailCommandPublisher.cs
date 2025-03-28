using Microsoft.Extensions.Logging;
using Loyalty.Application.Commands;
using Loyalty.Application.Services;
using Loyalty.Domain.Core;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.AuditTrailCommands;

public class AuditTrailCommandPublisher : KafkaPublisher<ICommand>, IAuditTrailCommandPublisher
{
    public AuditTrailCommandPublisher(
        IKafkaDispatcher producer,
        IApplicationCatalog applicationCatalog,
        IOmsAuditTrailScopedContext omsAuditTrailScopedContext,
        ILogger<KafkaPublisher<ICommand>> baseLogger
    ) : base(
        producer: producer,
        applicationCatalog: applicationCatalog,
        omsAuditTrailScopedContext: omsAuditTrailScopedContext,
        baseLogger: baseLogger
    )
    { }

    public Task PublishAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        => base.PublishAsync(Topics.OmsCoreAuditTrailCommandsPublic, command, cancellationToken);
}
