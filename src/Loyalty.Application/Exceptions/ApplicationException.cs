namespace Loyalty.Application.Exceptions;

public abstract class ApplicationException : Exception
{
    protected ApplicationException(string title, string message, Exception? innerException = null) : base(message, innerException)
    {
        Title = title;
    }

    public string Title { get; }
}
