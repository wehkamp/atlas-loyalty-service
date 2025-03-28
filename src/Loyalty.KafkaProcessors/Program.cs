using Loyalty.Application;
using Loyalty.Application.Services;
using Loyalty.Domain.Core;
using Loyalty.Infrastructure;
using Loyalty.Infrastructure.External.Kafka;
using Loyalty.Infrastructure.HealthChecks;
using Loyalty.Infrastructure.Lifetime;
using Loyalty.Infrastructure.Logging;
using Loyalty.Infrastructure.Settings;
using Loyalty.KafkaProcessors.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using System;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Loyalty.KafkaProcessors;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IOmsAuditTrailScopedContext, OmsAuditTrailScopedContext>();
        builder.Services.AddFraudHealthChecks();
        builder.Services.AddSettings(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddLogging();
        builder.Services.UseMinimalHttpLogger();
        // Workaround for the build logging to be clear. We need to clear all providers and set the minimum level to Warning to not spam the build log for the service tests.
        if (args.Any(argument => argument.Equals("--environment=UnitTests", StringComparison.InvariantCultureIgnoreCase)))
        {
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Warning);
        }

        builder.Services.AddHttpContextAccessor();
        builder.Host.UseOmsCoreLogging();

        builder.Services
            .AddControllers(o => o.ModelBinderProviders.Insert(0, new LocalDateTimeModelBinderProvider()))
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                o.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            })
            .AddXmlSerializerFormatters();
        
        var app = builder.Build();

        app.UseRouting();
        app.UsePrometheusMetrics();
        app.UseExceptionHandler(app.Logger);

#pragma warning disable ASP0014 // Suggest using top level route registrations
        app.UseEndpoints(endpointRouteBuilder =>
        {
            endpointRouteBuilder.MapControllers();
            endpointRouteBuilder.MapFraudHealthChecks();
            endpointRouteBuilder.MapMetrics(settings => settings.EnableOpenMetrics = false);
        });
#pragma warning restore ASP0014 // Suggest using top level route registrations

        // Get the consumer group from the settings so we know what group is logging
        var consumerGroup = app.Services.GetRequiredService<IOptions<KafkaSettings>>();
        app.AddOmsCoreKafkaLifetime(consumerGroup.Value.GroupName, consumerGroup.Value.ConfiguredTopics);

        using var scope = app.Services.CreateScope();

        var kafkaTopicHelper = scope.ServiceProvider.GetRequiredService<IKafkaTopicHelper>();
        var topicsExist = await kafkaTopicHelper.KafkaTopicsExists();

        if (!topicsExist)
        {
            app.Lifetime.StopApplication();
        }

        await app.RunAsync();
    }
}
