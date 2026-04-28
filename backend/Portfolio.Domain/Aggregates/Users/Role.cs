namespace Portfolio.Domain.Aggregates.Users;

public class Role
{
	#region Properties

	public Guid Id { get; private set; }
	public string Name { get; private set; } = string.Empty;

	#endregion

	#region Navigation Properties

	public virtual ICollection<Permission> Permissions { get; private set; } = [];
	public virtual ICollection<User> Users { get; private set; } = [];

	#endregion

	#region EF Core Constructor

	private Role()
	{
	}

	public Role(string name)
	{
		Id = Guid.NewGuid();
		Name = name;
	}

	public Role(Guid id, string name)
	{
		Id = id;
		Name = name;
	}

	#endregion


	#region Factory Methods
	public static Role Create(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Role name cannot be empty", nameof(name));

		return new Role(name);
	}

	public static Role Create(Guid id, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Role name cannot be empty", nameof(name));

		return new Role(id, name);
	}

	public static Role Create(string name, IEnumerable<Permission> permissions, IEnumerable<User> users)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Role name cannot be empty", nameof(name));

		var role = new Role(name);
		foreach (var permission in permissions)
		{
			role.AddPermission(permission);
		}
		foreach (var user in users)
		{
			role.AddUser(user);
		}
		return role;
	}

	#endregion

	#region Methods

	public void AddPermission(Permission permission)
	{
		if (Permissions.Any(p => p.Id == permission.Id))
			throw new InvalidOperationException("Permission already exists in this role.");

		Permissions.Add(permission);
	}

	public void RemovePermission(Guid permissionId)
	{
		var permission = Permissions.FirstOrDefault(p => p.Id == permissionId);
		if (permission == null)
			throw new InvalidOperationException("Permission not found in this role.");

		Permissions.Remove(permission);
	}

	public void AddUser(User user)
	{
		if (Users.Any(u => u.Id == user.Id))
			throw new InvalidOperationException("User already has this role.");

		Users.Add(user);
	}

	public void RemoveUser(Guid userId)
	{
		var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new InvalidOperationException("User not found in this role.");
		Users.Remove(user);
	}

	public void UpdateName(string newName)
	{
		if (string.IsNullOrWhiteSpace(newName))
			throw new ArgumentException("Role name cannot be empty", nameof(newName));

		Name = newName;
	}

	public void UpdatePermissions(IEnumerable<Permission> newPermissions)
	{
		Permissions.Clear();
		foreach (var permission in newPermissions)
		{
			AddPermission(permission);
		}
	}

	public void UpdateUsers(IEnumerable<User> newUsers)
	{
		Users.Clear();
		foreach (var user in newUsers)
		{
			AddUser(user);
		}
	}

	public void UpdateRole(string newName, IEnumerable<Permission> newPermissions, IEnumerable<User> newUsers)
	{
		UpdateName(newName);
		UpdatePermissions(newPermissions);
		UpdateUsers(newUsers);
	}

	public void ClearPermissions()
	{
		Permissions.Clear();
	}

	public void ClearUsers()
	{
		Users.Clear();
	}

	public void ClearRole()
	{
		ClearPermissions();
		ClearUsers();
	}

	#endregion
}