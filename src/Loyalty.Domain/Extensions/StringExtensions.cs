namespace Loyalty.Domain.Extensions;

public static class StringExtensions
{
    public static string? RemoveSuffix(this string? value, string suffix)
        => !string.IsNullOrWhiteSpace(value)
            ? value.EndsWith(suffix)
                ? value[..^suffix.Length]
                : value
            : null;
}
