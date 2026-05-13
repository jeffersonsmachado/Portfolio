namespace Portfolio.Application.Events;

public record class UserRegisteredEvent(Guid UserId, string Username);
