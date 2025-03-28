namespace Loyalty.Domain.Exceptions;

public class DomainException : InvalidOperationException
{
    public IEnumerable<string>? Errors { get; private set; }

    public DomainException()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, params string[]? errors) : base(message)
    {
        Errors = errors;
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
