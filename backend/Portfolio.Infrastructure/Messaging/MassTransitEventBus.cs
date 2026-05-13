using MassTransit;
using Portfolio.Application.Events;

namespace Portfolio.Infrastructure.Messaging;

public class MassTransitEventBus(IPublishEndpoint publishEndpoint) : IEventBus
{
	public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
	{
		await publishEndpoint.Publish(message, cancellationToken);
	}
}