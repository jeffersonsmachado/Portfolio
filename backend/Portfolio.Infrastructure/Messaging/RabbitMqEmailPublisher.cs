
using MassTransit;
using Portfolio.Application.Services;

namespace Portfolio.Infrastructure.Messaging;

public class RabbitMqEmailPublisher(IPublishEndpoint publishEndpoint) : IEmailService
{
	public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
	{
		await publishEndpoint.Publish(new EmailMessage
		{
			To = to,
			Subject = subject,
			Body = body
		}, cancellationToken);
	}
}