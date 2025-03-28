using Loyalty.Application.Models.Responses.AuditRecords;
using Loyalty.Domain.Core;

namespace Loyalty.Application.Queries.AuditRecords;

public record GetAuditRecordDetailsQuery : Query, IQuery<AuditDetailsResponse>
{
    public OrderNumber? OrderNumber { get; init; }
    public Guid DocumentId { get; init; }
    public required string Type { get; init; }
}
