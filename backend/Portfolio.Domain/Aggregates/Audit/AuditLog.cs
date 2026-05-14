namespace Portfolio.Domain.Aggregates.Audit;

public class AuditLog
{
	public Guid Id { get; set; }
	public string? Action { get; set; }
	public Guid? UserId { get; set; }
	public DateTime OccurredAt { get; set; }
}