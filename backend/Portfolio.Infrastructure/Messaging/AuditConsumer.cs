using MassTransit;
using Portfolio.Application.Events;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Messaging;

public class AuditConsumer(PortfolioDbContext portfolioDbContext) : IConsumer<AuditEvent>
{
	public async Task Consume(ConsumeContext<AuditEvent> context)
	{
		var auditEvent = context.Message;

		var auditLog = new Domain.Aggregates.Audit.AuditLog
		{
			Id = Guid.NewGuid(),
			Action = auditEvent.Action,
			UserId = auditEvent.UserId,
			OccurredAt = auditEvent.OccurredAt
		};

		portfolioDbContext.AuditLog.Add(auditLog);
		await portfolioDbContext.SaveChangesAsync();
	}
}