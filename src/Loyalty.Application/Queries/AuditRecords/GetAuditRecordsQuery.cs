using Loyalty.Application.Models;
using Loyalty.Application.Models.Responses.AuditRecords;
using Loyalty.Domain.Core;

namespace Loyalty.Application.Queries.AuditRecords;

public record GetAuditRecordsQuery : Query, IQuery<PageResponse<AuditRecordResponse>>
{
    public OrderNumber? OrderNumber { get; init; }
    public int? Skip { get; init; }
    public int? Take { get; init; }
    public string[]? AuditRecordTypes { get; init; }
}
