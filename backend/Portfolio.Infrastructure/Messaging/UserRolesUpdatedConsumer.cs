using MassTransit;
using Portfolio.Application.Profiles;
using Portfolio.Application.Events;
using Portfolio.Application.Services;

namespace Portfolio.Infrastructure.Messaging;

public class UserRolesUpdatedConsumer(ITokenRevocationService tokenRevocationService) : IConsumer<UserRolesUpdatedEvent>
{
	public async Task Consume(ConsumeContext<UserRolesUpdatedEvent> context)
	{
		var evt = context.Message;

		await tokenRevocationService.RevokeUserAsync(evt.UserId);
	}
}