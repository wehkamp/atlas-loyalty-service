namespace Loyalty.Infrastructure.External.Database.Context.Entities;

/// <summary>
/// The AuditDocumentMetadata entity will be used as a reference to the actual document stored in S3.
/// This entity is saved in the database.
/// </summary>
public class AuditDocumentMetadata
{
    public required Guid DocumentId { get; init; }
    public required string Type { get; init; }
    public required string Name { get; init; }
    public required DateTime Timestamp { get; init; }
    public string? Username { get; init; }
    public required Guid CorrelationId { get; init; }
    public string? OrderNumber { get; init; }
    public required string AtlasLabel { get; init; }
}
