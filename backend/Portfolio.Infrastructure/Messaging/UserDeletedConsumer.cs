using MassTransit;
using Portfolio.Application.Profiles;
using Portfolio.Application.Events;

namespace Portfolio.Infrastructure.Messaging;

public class UserDeletedConsumer(IProfileService profileService) : IConsumer<UserDeletedEvent>
{
	public async Task Consume(ConsumeContext<UserDeletedEvent> context)
	{
		var evt = context.Message;

		var profileResult = await profileService.GetProfileByUserIdAsync(evt.UserId, context.CancellationToken);

		if (profileResult.IsSuccess)
		{
			await profileService.DeleteAsync(profileResult.Value!.Id, context.CancellationToken);
		}


	}
}