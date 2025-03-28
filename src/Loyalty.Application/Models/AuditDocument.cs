namespace Loyalty.Application.Models;

public record AuditDocument
{
    public required Guid DocumentId { get; init; }
    public required string Type { get; init; }
    public required string Name { get; init; }
    public required DateTime Timestamp { get; init; }
    public string? Username { get; init; }
    public required Guid CorrelationId { get; init; }
    public long? OrderId { get; init; }
    public string? OrderNumber { get; init; }
}
