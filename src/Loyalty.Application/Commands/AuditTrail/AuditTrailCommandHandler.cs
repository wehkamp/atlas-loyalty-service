using Loyalty.Application.Services;
using Loyalty.Domain.Core;

namespace Loyalty.Application.Commands.AuditTrail;

public class AuditTrailCommandHandler : ICommandHandler<AuditTrailCommand>
{
    private readonly IOmsAuditTrailScopedContext _omsAuditTrailScopedContext;
    private readonly IAuditTrailDbCommandPublisher _auditTrailDbCommandPublisher;
    private readonly IAuditTrailS3CommandPublisher _auditTrailS3CommandPublisher;

    public AuditTrailCommandHandler(
        IOmsAuditTrailScopedContext omsAuditTrailScopedContext,
        IAuditTrailDbCommandPublisher auditTrailDbCommandPublisher,
        IAuditTrailS3CommandPublisher auditTrailS3CommandPublisher
    )
    {
        _omsAuditTrailScopedContext = omsAuditTrailScopedContext;
        _auditTrailDbCommandPublisher = auditTrailDbCommandPublisher;
        _auditTrailS3CommandPublisher = auditTrailS3CommandPublisher;
    }

    public async Task ExecuteAsync(AuditTrailCommand command, CancellationToken cancellationToken)
    {
        // Set up the commands, so we have automatic retries on both of them trough kafka consumption
        var documentId = Guid.NewGuid();
        var auditTrailDbCommand = new AuditTrailDbCommand
        {
            DocumentId = documentId,
            Type = command.Type,
            AuditName = command.AuditName,
            Timestamp = command.Timestamp,
            Username = command.Username,
            CorrelationId = command.CorrelationId,
            OrderNumber = command.OrderNumber,
            AtlasLabel = _omsAuditTrailScopedContext.AtlasLabel.Value,
        };
        var auditTrailS3Command = new AuditTrailS3Command
        {
            Type = command.Type,
            DocumentId = documentId,
            Serialized = command.Serialized,
            Timestamp = command.Timestamp,
            OrderNumber = command.OrderNumber,
            AtlasLabel = _omsAuditTrailScopedContext.AtlasLabel.Value,
        };

        // Publish both parts separate of each-other because they both will have their own process and handler
        await Task.WhenAll(
            _auditTrailDbCommandPublisher.PublishAsync(auditTrailDbCommand, cancellationToken),
            _auditTrailS3CommandPublisher.PublishAsync(auditTrailS3Command, cancellationToken)
        );
    }
}
