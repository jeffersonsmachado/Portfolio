namespace Portfolio.Application.Events;

public record AuditEvent
(
	string Action,
	Guid? UserId,
	DateTime OccurredAt
);