namespace Loyalty.Application.Exceptions;

public sealed class ValidationException : ApplicationException
{
    public ValidationException() : base("", "")
    {
        ErrorsDictionary = new Dictionary<string, string[]>();
    }

    public ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
        : base("Validation failure", ToMessage(errorsDictionary))
    {
        ErrorsDictionary = errorsDictionary;
    }

    public ValidationException(string[] errors)
        : this(new Dictionary<string, string[]> { { "Errors", errors } })
    {
    }

    public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; }

    private static string ToMessage(IReadOnlyDictionary<string, string[]> errorsDictionary)
    {
        return string.Join(
            Environment.NewLine,
            errorsDictionary.Select(kvp => $"{kvp.Key}:{Environment.NewLine}{string.Join(Environment.NewLine, kvp.Value.Select(e => $" - {e}"))}"));
    }
}
