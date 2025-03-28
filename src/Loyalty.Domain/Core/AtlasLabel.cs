using Loyalty.Domain.Core.StronglyTypedIds;
using System.Text.Json.Serialization;

namespace Loyalty.Domain.Core;

[JsonConverter(typeof(StringIdJsonConvertor<AtlasLabel>))]
public readonly record struct AtlasLabel(string Value) : IStringId
{ }
