namespace Loyalty.Domain.Core.StronglyTypedIds;

public interface IStronglyTypedId<TId>
{
    TId Value { get; }
}
