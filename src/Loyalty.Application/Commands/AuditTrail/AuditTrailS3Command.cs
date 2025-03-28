namespace Loyalty.Application.Commands.AuditTrail;

public record AuditTrailS3Command : AuditTrailBaseCommand
{
    public required string Type { get; init; }
    public Guid DocumentId { get; init; }
    public required string Serialized { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public string? OrderNumber { get; set; }
    public string? Username { get; init; }
}
