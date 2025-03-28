namespace Loyalty.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base($"Not Found: {message}")
    { }

    public NotFoundException(string entity, string id) : this($"{entity} with id {id} not found")
    { }

    public NotFoundException(string entity, long id) : this(entity, id.ToString())
    { }
}

public class ArticleGroupCodeNotFoundException : NotFoundException
{
    public ArticleGroupCodeNotFoundException(string message) : base(message)
    { }
}

public class TotalOrderAmountThresholdNotFoundException : NotFoundException
{
    public TotalOrderAmountThresholdNotFoundException() : base(string.Empty)
    { }

    public TotalOrderAmountThresholdNotFoundException(string message) : base(message)
    { }
}
