using Loyalty.Domain.Extensions;

namespace Loyalty.Application.Commands;

public abstract record Command : ICommand
{
    /// <summary>
    /// Unique identifier for the command
    /// </summary>
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Timestamp when the command was published
    /// </summary>
    public DateTime PublishedDateTime { get; init; } = DateTime.Now;

    /// <summary>
    /// Name of the command
    /// </summary>
    public string Name => GetType().Name;

    public string AtlasLabel { get; set; } = string.Empty;
}
