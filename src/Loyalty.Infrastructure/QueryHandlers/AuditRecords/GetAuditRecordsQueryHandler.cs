using Microsoft.EntityFrameworkCore;
using Loyalty.Application.Models;
using Loyalty.Application.Models.Responses.AuditRecords;
using Loyalty.Application.Queries;
using Loyalty.Application.Queries.AuditRecords;
using Loyalty.Infrastructure.External.Database.Context;

namespace Loyalty.Infrastructure.QueryHandlers.AuditRecords;

/// <summary>
/// Returns all audit records matching the given query
/// </summary>
public class GetAuditRecordsQueryHandler : IQueryHandler<GetAuditRecordsQuery, PageResponse<AuditRecordResponse>>
{
    private readonly IDbContextFactory<OmsCoreAuditTrailReaderDbContext> _omsDbReaderContextFactory;

    public GetAuditRecordsQueryHandler(IDbContextFactory<OmsCoreAuditTrailReaderDbContext> omsDbReaderContextFactory)
    {
        _omsDbReaderContextFactory = omsDbReaderContextFactory;
    }

    public async Task<PageResponse<AuditRecordResponse>> ExecuteAsync(GetAuditRecordsQuery query, CancellationToken cancellationToken)
    {
        await using var context = await _omsDbReaderContextFactory.CreateDbContextAsync(cancellationToken);

        var result = context.AuditDocumentMetadatas.TagWith(nameof(GetAuditRecordsQuery)).AsQueryable();
        if (query.OrderNumber.HasValue)
        {
            result = result.Where(r => r.OrderNumber == query.OrderNumber.Value.Value);

            if (query.AuditRecordTypes is not null)
            {
                result = result.Where(r => query.AuditRecordTypes.Contains(r.Type));
            }

            result = result.OrderByDescending(r => r.Timestamp);
        }

        var count = await result.CountAsync(cancellationToken);

        if (query.Skip.HasValue)
        {
            result = result.Skip(query.Skip.Value);
        }

        if (query.Take.HasValue)
        {
            result = result.Take(query.Take.Value);
        }

        var response = await result.ToListAsync(cancellationToken);

        var items = new AuditRecordResponse[0];
        
        return new PageResponse<AuditRecordResponse> { Items = items, Count = count };
    }
}
