using Loyalty.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using OMS.Common.Storage.AWS.S3bucket.Configuration.Options;

namespace Loyalty.Api.Core;

public static class SettingsExtensions
{
    public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        AddSettingsLocal<KafkaSettings>(services, configuration, "Kafka");
        AddSettingsLocal<AwsS3BucketOptions>(services, configuration, "Storage:S3Bucket");
    }

    private static void AddSettingsLocal<TSettings>(IServiceCollection services, IConfiguration configuration, string? key = null, Action<TSettings>? postConfigureOptions = null) where TSettings : class
    {
        OptionsBuilder<TSettings> settingsBuilder;

        if (string.IsNullOrWhiteSpace(key))
        {
            settingsBuilder = services
                .AddOptions<TSettings>()
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        else
        {
            var section = configuration.GetRequiredSection(key);

            settingsBuilder = services
                .AddOptions<TSettings>()
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        if (postConfigureOptions is not null)
        {
            settingsBuilder = settingsBuilder.PostConfigure(postConfigureOptions);
        }

        services.AddSingleton(c => c.GetRequiredService<IOptions<TSettings>>().Value);
    }
}
