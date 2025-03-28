using Loyalty.Domain.Core.StronglyTypedIds;
using System.Text.Json.Serialization;

namespace Loyalty.Domain.ArticleGroupCode;

[JsonConverter(typeof(StringIdJsonConvertor<ArticleGroupCodeId>))]
public readonly record struct ArticleGroupCodeId(string Value) : IStringId
{
}
