using MassTransit;
using Portfolio.Application.Profiles;
using Portfolio.Application.Events;

namespace Portfolio.Infrastructure.Messaging;

public class UserRegisteredConsumer(IProfileService profileService) : IConsumer<UserRegisteredEvent>
{
	public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
	{
		var evt = context.Message;

		await profileService.CreateAsync(new CreateProfileRequest
		{
			Name = evt.Username,
			UserId = evt.UserId
		}, context.CancellationToken);
	}
}