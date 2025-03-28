using Loyalty.Application.Exceptions;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Loyalty.Application;
public class DateTimeConverter : JsonConverter<DateTime>
{
    public DateTime Read(string? value)
    {
        var result = value is not null ? DateTime.Parse(value) : DateTime.MinValue;

        if (result.Kind == DateTimeKind.Unspecified)
        {
            throw new BadRequestException("DateTime without timezone is not allowed");
        }

        return result;
    }
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Read(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture));
    }
}
