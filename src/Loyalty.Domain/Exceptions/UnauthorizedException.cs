namespace Loyalty.Domain.Exceptions;

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(Type type)
        : base(
            title: "Not enough rights",
            message: $"Not allowed to execute {type.Name} because no principal was given"
        )
    { }

    public UnauthorizedException(string message)
        : base(
            title: "Action not allowed for sellerParty",
            message: message
        )
    { }
}
