using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Loyalty.Infrastructure.Lifetime;

public static class OmsCoreLifetimeExtensions
{
    public static void AddOmsCoreWebApiLifeTime(this WebApplication webApplication, string apiName)
    {
        webApplication.Lifetime.ApplicationStarted.Register(() =>
        {
            webApplication.Logger.LogInformation("OMS Core {apiName} started", apiName);
        });
        webApplication.Lifetime.ApplicationStopping.Register(() =>
        {
            webApplication.Logger.LogInformation("OMS Core {apiName} stopping", apiName);
        });
        webApplication.Lifetime.ApplicationStopped.Register(() =>
        {
            webApplication.Logger.LogInformation("OMS Core {apiName} stopped", apiName);
        });
    }

    public static void AddOmsCoreKafkaLifetime(this WebApplication webApplication, string consumerGroup, IList<string> topics)
    {
        var topicsString = string.Join(", ", topics);

        webApplication.Lifetime.ApplicationStarted.Register(() =>
        {
            webApplication.Logger.LogInformation("OMS Core KafkaProcessors started group: {consumerGroup} for topics: {topics}", consumerGroup, topicsString);
        });
        webApplication.Lifetime.ApplicationStopping.Register(() =>
        {
            webApplication.Logger.LogInformation("OMS Core KafkaProcessors stopping group: {consumerGroup} for topics: {topics}", consumerGroup, topicsString);
        });
        webApplication.Lifetime.ApplicationStopped.Register(() =>
        {
            webApplication.Logger.LogInformation("OMS Core KafkaProcessors stopped group: {consumerGroup} for topics: {topics}", consumerGroup, topicsString);
        });
    }
}
