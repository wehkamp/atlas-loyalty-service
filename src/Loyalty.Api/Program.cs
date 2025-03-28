using Loyalty.Api.Core;
using Loyalty.Api.Core.Middleware;
using Loyalty.Api.Core.Swagger;
using Loyalty.Application;
using Loyalty.Application.Services;
using Loyalty.Domain.Core;
using Loyalty.Infrastructure;
using Loyalty.Infrastructure.External.Kafka;
using Loyalty.Infrastructure.HealthChecks;
using Loyalty.Infrastructure.Lifetime;
using Loyalty.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Loyalty.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IOmsAuditTrailScopedContext, OmsAuditTrailScopedContext>();
        builder.Services.AddSwagger();
        builder.Services.AddFraudHealthChecks();
        builder.Services.AddSettings(builder.Configuration);
        builder.Services.AddApplication();
        // Override the unit tests due to we need to pass an argument with the unit tests 
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddLogging();
        builder.Services.UseMinimalHttpLogger();

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

        app.UseMiddleware<LabelIdMiddleware>();
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

        app.ConfigureSwaggerUI();

        app.AddOmsCoreWebApiLifeTime("Audit Trail Api");

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
