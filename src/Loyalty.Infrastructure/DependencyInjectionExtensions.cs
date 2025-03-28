using Loyalty.Application.Commands;
using Loyalty.Application.Services;
using Loyalty.Infrastructure.Decorators;
using Loyalty.Infrastructure.External.Database;
using Loyalty.Infrastructure.External.Kafka;
using Loyalty.Infrastructure.External.Kafka.Consumers.ConsumeResultProcessors;
using Loyalty.Infrastructure.External.Kafka.Consumers.PublicBusMessages;
using Loyalty.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Loyalty.Infrastructure.External.Kafka.Consumers.AuditTrailCommands;
using System.Reflection;

namespace Loyalty.Infrastructure;
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool runBackgroundServices = false)
    {
        services.AddSingleton<IApplicationCatalog, ApplicationCatalog>();

        // Command, event and query handlers
        services.AddCqrs();

        // Kafka
        services.AddKafkaProducers();
        services.AddKafkaConsumers();

        // Database
        services.AddDatabase(configuration);

        services.AddRepositories();

        // AWS S3 Bucket client & Storage services
        services.AddStorage();

        return services;
    }

    private static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        // Query Handlers
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.AssignableToAny(typeof(Application.Queries.IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        // Command handlers
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.AssignableToAny(typeof(IInMemoryCommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        // Command decorators
        services.TryDecorate(typeof(IInMemoryCommandHandler<,>), typeof(ValidatingCommandHandlerDecorator<,>));
        services.TryDecorate(typeof(IInMemoryCommandHandler<>), typeof(ValidatingCommandHandlerDecorator<>));

        return services;
    }

    private static IServiceCollection AddKafkaConsumers(this IServiceCollection services)
    {
        // Get the Kafka settings out of the configuration.
        var kafkaSettings = services.BuildServiceProvider().GetRequiredService<IOptions<KafkaSettings>>();

        if (!kafkaSettings.Value.ConfiguredTopics.Any())
        {
            // No topics configured. We don't need to consume anything
            return services;
        }

        services.AddScoped<IMessageConsumeResultProcessor, MessageConsumeResultProcessor>();

        services.AddScoped<IPublicBusMessageDispatcher, PublicBusMessageDispatcher>();

        if (kafkaSettings.Value.ConfiguredTopics.Contains(Topics.OmsCoreAuditTrailCommandsPublic))
        {
            services.AddHostedService<AuditTrailCommandConsumer>();
        }

        if (kafkaSettings.Value.ConfiguredTopics.Contains(Topics.OmsCoreAuditTrailPrivateDbCommands))
        {
            services.AddHostedService<AuditTrailDbCommandConsumer>();
        }

        if (kafkaSettings.Value.ConfiguredTopics.Contains(Topics.OmsCoreAuditTrailPrivateS3Commands))
        {
            services.AddHostedService<AuditTrailS3CommandConsumer>();
        }

        return services;
    }

    private static IServiceCollection AddKafkaProducers(this IServiceCollection services)
    {
        // Private Commands
        services.AddScoped<IAuditTrailCommandPublisher, AuditTrailCommandPublisher>();
        services.AddScoped<IAuditTrailDbCommandPublisher, AuditTrailDbCommandPublisher>();
        services.AddScoped<IAuditTrailS3CommandPublisher, AuditTrailS3CommandPublisher>();

        // Kafka topic helper
        services.AddSingleton<IKafkaTopicHelper, KafkaTopicHelper>();

        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddScoped<IKafkaDispatcher, KafkaDispatcher>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Repositories
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.AssignableToAny(typeof(Loyalty.Application.Repositories.IRepository)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }

    private static IServiceCollection AddStorage(this IServiceCollection services)
    {
        return services;
    }
}
