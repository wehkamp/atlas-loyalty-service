using FluentValidation;
using Loyalty.Application.Commands;
using Loyalty.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Loyalty.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Get this assembly
        var assembly = Assembly.GetExecutingAssembly();

        // In memory command handlers
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .FromAssemblies()
            .AddClasses(classes => classes.AssignableToAny(typeof(IInMemoryCommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        // Command handlers
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableToAny(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        // Validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
