namespace Loyalty.Application.Events;

public record Event : IEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Timestamp when the event was published
    /// </summary>
    public DateTime PublishedDateTime { get; init; } = DateTime.Now;

    /// <summary>
    /// Name of the event
    /// </summary>
    public string Name => GetType().Name;

    /// <summary>
    /// Company wide unique identifier for the label
    /// </summary>
    public string AtlasLabel { get; set; } = string.Empty;
}

