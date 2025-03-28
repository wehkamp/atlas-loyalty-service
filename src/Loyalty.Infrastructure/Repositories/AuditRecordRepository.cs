using Amazon.Runtime.Internal.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using OMS.Common.Mapper.ExtensionMethods;
using Loyalty.Application.Commands.AuditTrail;
using Loyalty.Application.Models;
using Loyalty.Application.Repositories;
using Loyalty.Domain.Core;
using Loyalty.Infrastructure.External.Database.Context;
using Loyalty.Infrastructure.External.Database.Context.Entities;

namespace Loyalty.Infrastructure.Repositories;

public class AuditRecordRepository : IAuditRecordRepository
{
    private readonly IDbContextFactory<OmsCoreAuditTrailReaderDbContext> _omsDbReaderContextFactory;
    private readonly IDbContextFactory<OmsCoreAuditTrailWriterDbContext> _omsDbWriterContextFactory;
    private readonly IOmsAuditTrailScopedContext _omsAuditTrailScopedContext;
    private readonly ILogger<AuditRecordRepository> _logger;

    public AuditRecordRepository(
        IDbContextFactory<OmsCoreAuditTrailReaderDbContext> omsDbReaderContextFactory,
        IDbContextFactory<OmsCoreAuditTrailWriterDbContext> omsDbWriterContextFactory,
        IOmsAuditTrailScopedContext omsAuditTrailScopedContext,
        ILogger<AuditRecordRepository> logger
    )
    {
        _omsDbReaderContextFactory = omsDbReaderContextFactory;
        _omsDbWriterContextFactory = omsDbWriterContextFactory;
        _omsAuditTrailScopedContext = omsAuditTrailScopedContext;
        _logger = logger;
    }

    public async Task<IEnumerable<AuditDocument>> GetByOrderIdAsync(OrderNumber orderNumber, CancellationToken cancellationToken)
    {
        await using var context = await _omsDbReaderContextFactory.CreateDbContextAsync(cancellationToken);

        var response = await context.AuditDocumentMetadatas
            .TagWith(nameof(AuditRecordRepository))
            .TagWith(nameof(GetByOrderIdAsync))
            .Where(r => r.OrderNumber == orderNumber.Value)
            .ToArrayAsync(cancellationToken);

        return response.MapTo<AuditDocument[]>();
    }

    public async Task PersistAsync(AuditTrailDbCommand auditTrailDbCommand, CancellationToken cancellationToken)
    {
        await using var context = await _omsDbWriterContextFactory.CreateDbContextAsync(cancellationToken);

        try
        {
            await context.WithChangeTrackingTransactionAsync(async () =>
            {
                var auditDocumentMetadata = new AuditDocumentMetadata
                {
                    DocumentId = auditTrailDbCommand.DocumentId,
                    Type = auditTrailDbCommand.Type,
                    Name = auditTrailDbCommand.AuditName,
                    Timestamp = auditTrailDbCommand.Timestamp,
                    Username = auditTrailDbCommand.Username,
                    CorrelationId = auditTrailDbCommand.CorrelationId,
                    OrderNumber = auditTrailDbCommand.OrderNumber,
                    AtlasLabel = _omsAuditTrailScopedContext.AtlasLabel.Value
                };

                context.AuditDocumentMetadatas.Add(auditDocumentMetadata);

                await context.SaveChangesAsync(cancellationToken);

                return auditDocumentMetadata;
            }, cancellationToken);
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is PostgresException { SqlState: "23505" })
        {
            // Handle duplicate key violation (SQLSTATE 23505)
            _logger.LogWarning("Trying to save a record that already exists with id: {documentId}", auditTrailDbCommand.DocumentId);
        }
    }

    public async Task RemoveAsync(AuditDocument auditRecord, CancellationToken cancellationToken)
    {
        await using var context = await _omsDbWriterContextFactory.CreateDbContextAsync(cancellationToken);

        await context.WithChangeTrackingTransactionAsync(async () =>
        {
            var auditDocumentMetadata = await context.AuditDocumentMetadatas
                .TagWith(nameof(AuditRecordRepository))
                .TagWith(nameof(RemoveAsync))
                .FirstOrDefaultAsync(r => r.DocumentId == auditRecord.DocumentId, cancellationToken: cancellationToken);

            if (auditDocumentMetadata != null)
            {
                context.AuditDocumentMetadatas.Remove(auditDocumentMetadata);
            }

            await context.SaveChangesAsync(cancellationToken);

            return auditRecord;
        }, cancellationToken);
    }

    public async Task RemoveManyAsync(IEnumerable<AuditDocument> auditRecords, CancellationToken cancellationToken)
    {
        await using var context = await _omsDbWriterContextFactory.CreateDbContextAsync(cancellationToken);

        await context.WithChangeTrackingTransactionAsync(async () =>
        {
            var documentIds = auditRecords.Select(r => r.DocumentId).ToArray();

            var auditDocumentMetadatas = await context.AuditDocumentMetadatas
                .TagWith(nameof(AuditRecordRepository))
                .TagWith(nameof(RemoveManyAsync))
                .Where(r => documentIds.Contains(r.DocumentId))
                .ToArrayAsync(cancellationToken);

            context.AuditDocumentMetadatas
                .RemoveRange(auditDocumentMetadatas);

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }, cancellationToken);
    }
}
