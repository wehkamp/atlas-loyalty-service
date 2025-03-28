using Microsoft.AspNetCore.Builder;
using Prometheus;
using System.Security.Claims;

namespace Loyalty.Api.Core;

public static class MetricsExtensions
{
    public static void UsePrometheusMetrics(this WebApplication app)
    {
        app.UseHttpMetrics(opt => opt.AddCustomLabel("client_id", context => context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "anonymous"));
    }
}
