namespace Loyalty.Application.Exceptions;

public class BadRequestException : ApplicationException
{
    public BadRequestException() : base("", "")
    { }

    public BadRequestException(string message) : base("Bad Request", message)
    { }
}
