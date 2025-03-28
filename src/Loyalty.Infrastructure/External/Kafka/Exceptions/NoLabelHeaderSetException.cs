using System.Text;

namespace Loyalty.Infrastructure.External.Kafka.Exceptions;

public class NoLabelHeaderSetException : Exception
{
    private static string CreateMessage(string header1, string? header2)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("No ");
        stringBuilder.Append(header1);
        stringBuilder.Append(' ');

        if (!string.IsNullOrWhiteSpace(header2))
        {
            stringBuilder.Append("or ");
            stringBuilder.Append(header2);
        }

        stringBuilder.Append("headers where set");

        return stringBuilder.ToString();
    }

    public NoLabelHeaderSetException(string header1, string? header2)
        : base(CreateMessage(header1, header2))
    { }
}
