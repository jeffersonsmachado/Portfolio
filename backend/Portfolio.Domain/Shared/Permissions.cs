namespace Portfolio.Domain.Shared;

public static class Permissions
{
	public const string ProfileView = "profile:view";
	public const string ProfileCreate = "profile:create";
	public const string ProfileUpdate = "profile:update";
	public const string ProfileDelete = "profile:delete";

	public const string UserView = "user:view";
	public const string UserCreate = "user:create";
	public const string UserUpdate = "user:update";
	public const string UserDelete = "user:delete";

	public const string RolesView = "roles:view";
	public const string RolesCreate = "roles:create";
	public const string RolesUpdate = "roles:update";
	public const string RolesDelete = "roles:delete";

}