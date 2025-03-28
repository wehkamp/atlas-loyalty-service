using Confluent.Kafka;
using Loyalty.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Loyalty.Infrastructure.External.Kafka;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private bool _disposed;

    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings)
    {
        _producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            SecurityProtocol = kafkaSettings.Value.UseSsl ? SecurityProtocol.Ssl : SecurityProtocol.Plaintext
        }).Build();
    }

    ~KafkaProducer()
    {
        Dispose(false);
    }

    public async Task<bool> ProduceAsync(string topic, Message<string, string> message, CancellationToken cancellationToken)
    {
        var result = await _producer.ProduceAsync(topic, message, cancellationToken);
        return result.Status == PersistenceStatus.Persisted;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _producer.Flush();
            _producer.Dispose();
        }

        _disposed = true;
    }
}
