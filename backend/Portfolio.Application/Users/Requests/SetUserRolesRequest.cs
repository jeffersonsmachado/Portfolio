namespace Portfolio.Application.Users.Requests;

public record SetUserRolesRequest(List<Guid> RoleIds);
