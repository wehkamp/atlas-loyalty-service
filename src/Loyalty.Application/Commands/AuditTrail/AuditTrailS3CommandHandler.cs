using Microsoft.Extensions.Logging;
using OMS.Common.Storage.AWS.S3bucket.Clients;
using OMS.Common.Storage.AWS.S3bucket.Models;
using Loyalty.Domain.Core;

namespace Loyalty.Application.Commands.AuditTrail;

public class AuditTrailS3CommandHandler : ICommandHandler<AuditTrailS3Command>
{
    private readonly IOmsAuditTrailScopedContext _omsAuditTrailScopedContext;
    private readonly IAwsS3BucketClient _awsS3BucketClient;
    private readonly ILogger<AuditTrailS3CommandHandler> _logger;

    public AuditTrailS3CommandHandler(
        IOmsAuditTrailScopedContext omsAuditTrailScopedContext,
        IAwsS3BucketClient awsS3BucketClient,
        ILogger<AuditTrailS3CommandHandler> logger
    )
    {
        _omsAuditTrailScopedContext = omsAuditTrailScopedContext;
        _awsS3BucketClient = awsS3BucketClient;
        _logger = logger;
    }

    private string FormatBucketDirectoryName(string type)
    {
        // We want to store the audit documents in a separate directory per AtlasLabel per type
        return $"{_omsAuditTrailScopedContext.AtlasLabel}/{type}";
    }

    public async Task ExecuteAsync(AuditTrailS3Command command, CancellationToken cancellationToken)
    {
        var bucketDocument = new BucketDocument
        {
            DocumentId = command.DocumentId,
            Document = command.Serialized,
            Metadata = new Dictionary<string, string>()
            {
                { "Timestamp", command.Timestamp.ToString() },
            }
        };

        if (!string.IsNullOrWhiteSpace(command.OrderNumber))
        {
            bucketDocument.Metadata.Add("OrderNumber", command.OrderNumber);
        }

        if (!string.IsNullOrWhiteSpace(command.Username))
        {
            bucketDocument.Metadata.Add("Username", command.Username);
        }

        try
        {
            _logger.LogInformation($"Storing the document in S3. DocumentId: {bucketDocument.DocumentId}");
            await _awsS3BucketClient.StoreDocumentAsync(FormatBucketDirectoryName(command.Type), bucketDocument, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to store the document in S3. DocumentId: {bucketDocument.DocumentId}");
            throw;
        }
    }
}
