using System.Collections.Immutable;

namespace Loyalty.Infrastructure;

public interface IApplicationCatalog
{
    ImmutableArray<Type> Events { get; }

    ImmutableArray<Type> Queries { get; }

    ImmutableArray<Type> InMemoryCommands { get; }

    ImmutableArray<Type> Commands { get; }

    Type? GetEventTypeByName(string eventName);

    Type? GetCommandTypeByName(string commandName);

    string GetEventName(Type eventType);

    string GetEventHandlerName(Type eventHandlerType);
}
