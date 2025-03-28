namespace Loyalty.Domain.Exceptions;

public class ConflictException : ApplicationException
{
    public ConflictException(string message) : base(message)
    { }
}

public class ArticleGroupCodeAlreadyExistsException : ConflictException
{
    public ArticleGroupCodeAlreadyExistsException() : base(string.Empty)
    { }

    public ArticleGroupCodeAlreadyExistsException(string message) : base(message)
    { }
}

public class TotalOrderAmountThresholdAlreadyExistsException : ConflictException
{
    public TotalOrderAmountThresholdAlreadyExistsException() : base(string.Empty)
    { }

    public TotalOrderAmountThresholdAlreadyExistsException(string message) : base(message)
    { }
}

public class CustomerAlreadyExistsException : ConflictException
{
    public CustomerAlreadyExistsException() : base(string.Empty)
    { }

    public CustomerAlreadyExistsException(string message) : base(message)
    { }
}
