using Loyalty.Domain.Core.StronglyTypedIds;
using System.Text.Json.Serialization;

namespace Loyalty.Domain.Core;

[JsonConverter(typeof(StringIdJsonConvertor<OrderNumber>))]
public readonly record struct OrderNumber : IStringId
{
    public static readonly OrderNumber Empty = new OrderNumber(string.Empty);

    public OrderNumber(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public override string ToString() => Value;

    public static implicit operator string(OrderNumber orderNumber) => orderNumber.Value;
}
