namespace Loyalty.Domain.Core.Extensions;

public static class DictionaryExtensions
{
    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
    {
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public static TValue GetOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFactory)
    {
        if (!dict.TryGetValue(key, out var val))
        {
            val = dict[key] = valueFactory();
        }

        return val;
    }
}
