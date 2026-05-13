namespace Portfolio.Application.Events;

public interface IEventBus
{
	Task PublishAsync<T>(T message, CancellationToken cancellationToken);
}