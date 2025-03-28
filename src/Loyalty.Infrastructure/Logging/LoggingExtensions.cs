using Microsoft.Extensions.Hosting;
using Loyalty.Domain.Core;
using Loyalty.Domain.Extensions;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
#if !DEBUG
using Microsoft.EntityFrameworkCore;
using Serilog.Filters;
using Serilog.Formatting.Elasticsearch;
using Serilog.Formatting.Json;
#endif

namespace Loyalty.Infrastructure.Logging;

public static class LoggingExtensions
{
    public static IHostBuilder UseOmsCoreLogging(this IHostBuilder builder)
        => builder.UseSerilog((context, services, configuration) =>
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            // Default logging settings
            configuration
                .Enrich.With<DemystifiedStackTraceEnricher>()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                .Enrich.FromLogContext()
                .Enrich.With<LogInformationEnricher>();

            // Add prometheus
            configuration.WriteTo.Prometheus();

#if DEBUG
            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.Debug();
#else
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Filter.ByExcluding(logEvent => logEvent.Exception is DbUpdateConcurrencyException)
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"))
            .WriteTo.Console(new ElasticsearchJsonFormatter(inlineFields: true));
#endif
        });

    public static IDisposable BeginOmsCoreLogScope<T>(this Microsoft.Extensions.Logging.ILogger logger, T obj, string objectTypeLogKey, IOmsAuditTrailScopedContext omsAuditTrailScopedContext, IDictionary<string, object>? loggingProperties = null)
    {
        var state = new Dictionary<string, object>
        {
            { "correlationId", omsAuditTrailScopedContext.CorrelationId.ToString() },
            { "atlas-label", omsAuditTrailScopedContext.AtlasLabel.ToString() },
            { objectTypeLogKey, typeof(T).SanitizeTypeToName() }
        };
        
        if (loggingProperties is not null)
        {
            foreach (var (key, value) in loggingProperties)
            {
                // Use a try add to avoid overwriting the state with the same key
                state.TryAdd(key, value);
            }
        }

        return logger.BeginScope(state)!;
    }

    private static readonly ConcurrentDictionary<Type, string> _friendlyNameNameCache = new ConcurrentDictionary<Type, string>();

    public static string SanitizeTypeToName(this Type type)
    {
        return _friendlyNameNameCache.GetOrAdd(type,
            t => t.Name.RemoveSuffix("Command")!.RemoveSuffix("Event")!.RemoveSuffix("Query")!);
    }
}

internal class LogInformationEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(new LogEventProperty("role", new ScalarValue("oms-core-audit-trail")));
        logEvent.AddPropertyIfAbsent(new LogEventProperty("team", new ScalarValue("order-management")));
    }
}

internal class DemystifiedStackTraceEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.Exception?.Demystify();
    }
}
