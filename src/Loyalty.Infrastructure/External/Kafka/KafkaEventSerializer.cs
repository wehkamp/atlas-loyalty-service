using Loyalty.Application;
using Loyalty.Application.Commands;
using Loyalty.Application.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Loyalty.Infrastructure.External.Kafka;
public static class KafkaEventSerializer
{
    private static readonly JsonSerializerOptions? _options;

    static KafkaEventSerializer()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new JsonStringEnumConverter());
        _options.Converters.Add(new DateTimeConverter());
    }

    public static string SerializeEvent<T>(T @event) where T : IEvent
        => JsonSerializer.Serialize(@event, @event.GetType(), _options);

    public static string SerializeCommand<T>(T command) where T : ICommand
        => JsonSerializer.Serialize(command, command.GetType(), _options);

    public static IEvent? DeserializeEvent(string serialized, Type type)
        => JsonSerializer.Deserialize(serialized, type, _options) as IEvent;

    public static ICommand? DeserializeCommand(string serialized, Type type)
        => JsonSerializer.Deserialize(serialized, type, _options) as ICommand;
}
