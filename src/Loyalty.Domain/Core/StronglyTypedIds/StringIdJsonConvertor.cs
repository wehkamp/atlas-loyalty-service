using System.Text.Json;
using System.Text.Json.Serialization;

namespace Loyalty.Domain.Core.StronglyTypedIds;

public class StringIdJsonConvertor<TId> : JsonConverter<TId> where TId : IStringId, new()
{
    public override TId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => (TId?)Activator.CreateInstance(typeToConvert, reader.GetString());

    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
