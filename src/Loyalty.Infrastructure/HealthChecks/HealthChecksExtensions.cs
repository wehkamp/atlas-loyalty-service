using Loyalty.Infrastructure.External.Database.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Loyalty.Infrastructure.HealthChecks;
public static class HealthChecksExtensions
{
    public static IServiceCollection AddFraudHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            //.AddCheck<OmsCoreDbContextHealthCheck<OmsCoreAuditTrailReaderDbContext>>(nameof(OmsCoreAuditTrailReaderDbContext))
            //.AddCheck<OmsCoreDbContextHealthCheck<OmsCoreAuditTrailWriterDbContext>>(nameof(OmsCoreAuditTrailWriterDbContext))
            .AddCheck<KafkaHealthCheck>("Kafka");

        return services;
    }

    public static IEndpointRouteBuilder MapFraudHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("startz");

        // Move the readyness probe check to just an OK
        endpoints.MapGet("/readyz", () => HealthCheckResult.Healthy("OK"))
            .ExcludeFromDescription();

        return endpoints;
    }
}
