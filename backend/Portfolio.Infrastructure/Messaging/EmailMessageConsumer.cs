using MassTransit;
using Portfolio.Infrastructure.Services;

namespace Portfolio.Infrastructure.Messaging;

public class EmailMessageConsumer(SmtpEmailService smtpEmailService) : IConsumer<EmailMessage>
{
	public async Task Consume(ConsumeContext<EmailMessage> context)
	{
		var message = context.Message;

		await smtpEmailService.SendEmailAsync(
			message.To!,
			message.Subject!,
			message.Body!,
			context.CancellationToken
		);
	}
}