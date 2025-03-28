using Loyalty.Application.Commands;
using Loyalty.Application.Events;
using Loyalty.Application.Queries;
using Loyalty.Domain.Extensions;
using System.Collections.Immutable;

namespace Loyalty.Infrastructure;

public class ApplicationCatalog : IApplicationCatalog
{
    private readonly ImmutableArray<Type> _events;
    private readonly ImmutableArray<Type> _commands;
    private readonly ImmutableArray<Type> _queries;
    private readonly ImmutableArray<Type> _inMemoryCommands;

    private readonly Dictionary<string, Type> _eventsByName;
    private readonly Dictionary<string, Type> _commandsByName;
    
    public ApplicationCatalog()
    {
        var inMemoryCommandType = typeof(IInMemoryCommand);
        var queryType = typeof(IQuery);
        var eventType = typeof(IEvent);
        var commandType = typeof(ICommand);

        var allTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && !t.Name.EndsWith("Decorator"))
            .ToArray();

        // Set events
        _events = allTypes
            .Where(eventType.IsAssignableFrom)
            .ToImmutableArray();
        _eventsByName = _events.ToDictionary(catalogEventType => catalogEventType.Name);

        // Set commands
        _commands = allTypes
            .Where(commandType.IsAssignableFrom)
            .ToImmutableArray();
        _commandsByName = _commands.ToDictionary(catalogCommandType => catalogCommandType.Name);

        _inMemoryCommands = allTypes
            .Where(inMemoryCommandType.IsAssignableFrom)
            .ToImmutableArray();

        _queries = allTypes
            .Where(queryType.IsAssignableFrom)
            .ToImmutableArray();
    }
    
    public ImmutableArray<Type> Events => _events;

    public ImmutableArray<Type> Queries => _queries;

    public ImmutableArray<Type> InMemoryCommands => _inMemoryCommands;

    public ImmutableArray<Type> Commands => _commands;

    public Type? GetEventTypeByName(string eventName)
    {
        if (_eventsByName.TryGetValue($"{eventName}Event", out var eventType))
        {
            return eventType;
        }

        return _eventsByName[eventName];
    }

    public Type? GetCommandTypeByName(string commandName)
    {
        if (_commandsByName.TryGetValue($"{commandName}Command", out var commandType))
        {
            return commandType;
        }

        return _commandsByName[commandName];
    }

    public string GetEventName(Type eventType) => eventType.Name;

    public string GetEventHandlerName(Type eventHandlerType)
    {
        Type? baseType = eventHandlerType.BaseType;
        if (baseType is not null && !baseType.IsAbstract && baseType.IsClass && baseType.IsAssignableTo(typeof(IEventHandler<>)))
        {
            return GetEventHandlerName(baseType);
        }

        return eventHandlerType.Name.RemoveSuffix("EventHandler")!;
    }
}
