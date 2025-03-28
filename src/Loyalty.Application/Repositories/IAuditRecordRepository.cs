using Loyalty.Application.Commands.AuditTrail;
using Loyalty.Application.Models;
using Loyalty.Domain.Core;

namespace Loyalty.Application.Repositories;

public interface IAuditRecordRepository : IRepository
{
    Task<IEnumerable<AuditDocument>> GetByOrderIdAsync(OrderNumber orderNumber, CancellationToken cancellationToken);
    Task PersistAsync(AuditTrailDbCommand auditTrailDbCommand, CancellationToken cancellationToken);
    Task RemoveAsync(AuditDocument auditDocument, CancellationToken cancellationToken);
    Task RemoveManyAsync(IEnumerable<AuditDocument> auditDocuments, CancellationToken cancellationToken);
}
