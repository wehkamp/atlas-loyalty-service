namespace Loyalty.Application.Models.Responses.AuditRecords;

public class AuditDetailsResponse
{
    public Guid DocumentId { get; init; }
    public required string Type { get; init; }
    public required string Serialized { get; init; }
}
