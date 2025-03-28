using Microsoft.Extensions.Logging;
using Loyalty.Application.Repositories;

namespace Loyalty.Application.Commands.AuditTrail;

public class AuditTrailDbCommandHandler : ICommandHandler<AuditTrailDbCommand>
{
    private readonly IAuditRecordRepository _auditRecordRepository;
    private readonly ILogger<AuditTrailDbCommandHandler> _logger;

    public AuditTrailDbCommandHandler(
        IAuditRecordRepository auditRecordRepository,
        ILogger<AuditTrailDbCommandHandler> logger
    )
    {
        _auditRecordRepository = auditRecordRepository;
        _logger = logger;
    }

    public Task ExecuteAsync(AuditTrailDbCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Persisting audit record. DocumentId: {command.DocumentId}");
            return _auditRecordRepository.PersistAsync(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to persist audit record. DocumentId: {command.DocumentId}");
            throw;
        }
    }
}
