using Loyalty.Application.Events;
using Loyalty.Application.Services;

namespace Loyalty.Infrastructure.External.Kafka.Consumers.PublicBusMessages;

public interface IPublicBusMessageDispatcher : IScopedService
{
    Task DispatchAsync(IBusMessage busMessage, CancellationToken cancellationToken);
}
