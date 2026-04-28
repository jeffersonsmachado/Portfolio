namespace Portfolio.Domain.Aggregates.Users;

public class Permission
{
	#region Properties

	public Guid Id { get; private set; }
	public string Name { get; private set; } = string.Empty;

	#endregion

	#region Navigation Properties

	public virtual ICollection<Role> Roles { get; private set; } = [];

	#endregion

	#region EF Core Constructor

	private Permission()
	{
	}

	public Permission(string name)
	{
		Id = Guid.NewGuid();
		Name = name;
	}

	public Permission(Guid id, string name)
	{
		Id = id;
		Name = name;
	}

	#endregion

	#region Factory Methods

	public static Permission Create(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Permission name cannot be empty", nameof(name));

		return new Permission(name);
	}

	public static Permission Create(Guid id, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Permission name cannot be empty", nameof(name));

		return new Permission(id, name);
	}

	public static Permission Create(string name, Role role)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Permission name cannot be empty", nameof(name));

		var permission = new Permission(name);
		role.AddPermission(permission);
		return permission;
	}

	#endregion

	#region Methods

	public void AddToRole(Role role)
	{
		if (Roles.Any(r => r.Id == role.Id))
			throw new InvalidOperationException("Permission already assigned to this role.");

		Roles.Add(role);
	}

	public void RemoveFromRole(Role role)
	{
		if (!Roles.Any(r => r.Id == role.Id))
			throw new InvalidOperationException("Permission not assigned to this role.");

		Roles.Remove(role);
	}

	public bool IsAssignedToRole(Guid roleId)
	{
		return Roles.Any(r => r.Id == roleId);
	}

	#endregion
}