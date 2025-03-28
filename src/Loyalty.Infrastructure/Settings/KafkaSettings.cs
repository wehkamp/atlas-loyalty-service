using System.ComponentModel.DataAnnotations;

namespace Loyalty.Infrastructure.Settings;
public record KafkaSettings
{
    [Required]
    public required string BootstrapServers { get; init; }
    public bool UseSsl { get; init; } = false;
    [Required]
    public required string GroupName { get; init; }
    [Required]
    public required string SchemaRegistryUrl { get; init; }
    public int RetryDelaySecondsFirst { get; init; } = 600;
    public int RetryDelaySecondsSecond { get; init; } = 600;
    public int RetryDelaySecondsThird { get; init; } = 600;
    public bool EnableConsumers { get; init; } = true;
    public List<string> ConfiguredTopics { get; init; } = new List<string>();
}
