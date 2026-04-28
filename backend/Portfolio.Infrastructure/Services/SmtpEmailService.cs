using System.Net.Mail;
using Portfolio.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Portfolio.Infrastructure.Services;

public class SmtpEmailService(IConfiguration configuration) : IEmailService
{
	private readonly string _host = configuration["Smtp:Host"] ?? "localhost";
	private readonly int _port = int.Parse(configuration["Smtp:Port"] ?? "1025");
	private readonly string _fromEmail = configuration["Smtp:FromEmail"] ?? "no-reply@portfolio.com";

	public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
	{

		using var client = new SmtpClient(_host, _port);

		var mailMessage = new MailMessage
		{
			From = new MailAddress(_fromEmail),
			Subject = subject,
			Body = body,
			IsBodyHtml = true
		};

		mailMessage.To.Add(to);

		await client.SendMailAsync(mailMessage, cancellationToken);
	}
}