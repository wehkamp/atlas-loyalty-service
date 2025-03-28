namespace Loyalty.Application.Exceptions;

public class ConcurrencyException : ApplicationException
{
    public ConcurrencyException(string title, string message, Exception? innerException = null) : base(title, message, innerException)
    {
    }

    public ConcurrencyException(Exception? innerException = null) : this("Concurrency failure", "A concurrency exception occurred", innerException)
    {
    }

    public static ConcurrencyException For<T>(uint expectedVersion, uint actualVersion) =>
        new ConcurrencyException($"Concurrency exception {typeof(T).Name}",
            $"Expected version {expectedVersion}, actual version {actualVersion}");
}
