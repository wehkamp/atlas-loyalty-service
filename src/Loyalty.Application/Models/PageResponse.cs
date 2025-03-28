namespace Loyalty.Application.Models;

public record struct PageResponse<T>
{
    /// <summary>
    /// Items in this paged response
    /// </summary>
    public T[] Items { get; init; }
    /// <summary>
    /// Total number of items
    /// </summary>
    public long Count { get; init; }
}
