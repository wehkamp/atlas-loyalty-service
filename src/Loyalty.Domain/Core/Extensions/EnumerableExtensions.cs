namespace Loyalty.Domain.Core.Extensions;
public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> list, Action<T> block)
    {
        foreach (var item in list)
        {
            block(item);
        }
    }
}
