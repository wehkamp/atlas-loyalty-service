namespace Loyalty.Application.Events;

/// <summary>
/// Describe a fact that has happened in the past, don't need to be handled real time and hence can scale well, and can be handled by many subscribers in many different ways.
/// </summary>
public interface IBusMessage
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    string Id { get; }
    /// <summary>
    /// Timestamp defining when the event was published
    /// </summary>
    DateTime PublishedDateTime { get; }

    /// <summary>
    /// Name of the event
    /// </summary>
    public string Name { get; }

    public string AtlasLabel { get; }
}

