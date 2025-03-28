namespace Loyalty.Application.Commands.AuditTrail;

public record AuditTrailDbCommand : AuditTrailBaseCommand
{
    public Guid DocumentId { get; init; }
    public required string Type { get; init; }
    public required string AuditName { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public string? Username { get; init; }
    public Guid CorrelationId { get; init; }
    public string? OrderNumber { get; set; }
    public string? Error { get; set; }
}
