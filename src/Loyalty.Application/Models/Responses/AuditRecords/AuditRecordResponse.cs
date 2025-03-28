namespace Loyalty.Application.Models.Responses.AuditRecords;

public class AuditRecordResponse
{
    public Guid DocumentId { get; init; }

    [Obsolete("This property is obsolete and will be removed in the future. Use DocumentId instead.")]
    public long? AuditRecordId { get; init; }
    public required string Type { get; init; }
    public required string Name { get; init; }
    public string? OrderNumber { get; init; }
    public DateTime Timestamp { get; init; }
    public string? Username { get; init; }
    public required string CorrelationId { get; init; }
    public string? Error { get; init; }
}
