namespace Loyalty.Domain.Exceptions;

public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message, Exception? innerException = null) : base(message, innerException)
    {
        Title = string.Empty;
    }

    protected ApplicationException(string title, string message, Exception? innerException = null) : base(message, innerException)
    {
        Title = title;
    }

    public string Title { get; }
}
