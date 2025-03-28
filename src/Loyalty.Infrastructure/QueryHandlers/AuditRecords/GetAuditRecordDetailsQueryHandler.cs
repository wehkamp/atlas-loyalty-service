using OMS.Common.Storage.AWS.S3bucket.Clients;
using OMS.Common.Storage.AWS.S3bucket.Models;
using Loyalty.Application.Models.Responses.AuditRecords;
using Loyalty.Application.Queries;
using Loyalty.Application.Queries.AuditRecords;
using Loyalty.Domain.Core;

namespace Loyalty.Infrastructure.QueryHandlers.AuditRecords;
public class GetAuditRecordDetailsQueryHandler : IQueryHandler<GetAuditRecordDetailsQuery, AuditDetailsResponse>
{
    private readonly IAwsS3BucketClient _awsS3BucketClient;
    private readonly IOmsAuditTrailScopedContext _omsAuditTrailScopedContext;

    public GetAuditRecordDetailsQueryHandler(
        IAwsS3BucketClient awsS3BucketClient,
        IOmsAuditTrailScopedContext omsAuditTrailScopedContext
    )
    {
        _awsS3BucketClient = awsS3BucketClient;
        _omsAuditTrailScopedContext = omsAuditTrailScopedContext;
    }

    public async Task<AuditDetailsResponse> ExecuteAsync(GetAuditRecordDetailsQuery query, CancellationToken cancellationToken)
    {
        BucketDocument? result;

        result = await _awsS3BucketClient.RetrieveDocumentAsync(FormatBucketDirectoryName(query.Type), query.DocumentId, cancellationToken);

        return new AuditDetailsResponse
        {
            DocumentId = result.DocumentId,
            Serialized = result.Document,
            Type = query.Type
        };
    }

    private string FormatBucketDirectoryName(string type)
    {
        return $"{_omsAuditTrailScopedContext.AtlasLabel}/{type}";
    }
}
