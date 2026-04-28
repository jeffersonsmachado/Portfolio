using Portfolio.Domain.Aggregates.Users;

namespace Portfolio.Domain.Aggregates.Profiles;

public partial class Profile
{
	#region Properties

	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public string Bio { get; private set; } = string.Empty;
	public string BioTitle { get; private set; } = string.Empty;
	public string AvatarUrl { get; private set; } = string.Empty;

	#endregion

	#region Navigation Properties

	public virtual User User { get; set; } = null!;

	#endregion

	#region EF Core Constructor
	private Profile()
	{
		Name = string.Empty;
	}

	private Profile(User user)
	{
		Name = string.Empty;
		User = user;
	}

	private Profile(string name, User user)
	{
		Id = Guid.NewGuid();
		Name = name;
		User = user;
	}

	public static Profile Create(string name, User user)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Profile name cannot be empty", nameof(name));
		}
		return new Profile(name, user);
	}

	public void UpdateProfile(string name, string? bio, string? bioTitle, string? avatarUrl)
	{
		Name = name;
		Bio = bio ?? string.Empty;
		BioTitle = bioTitle ?? string.Empty;
		AvatarUrl = avatarUrl ?? string.Empty;
	}

	#endregion

	#region Factory Methods

	// Factory method for recreating from storage (or mock data)
	public static Profile Recreate(Guid id, string name, User user, string bio = "", string bioTitle = "", string avatarUrl = "")
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException("Id cannot be empty", nameof(id));
		}

		return new Profile(name, user)
		{
			Id = id,
			Bio = bio,
			BioTitle = bioTitle,
			AvatarUrl = avatarUrl
		};
	}

	#endregion
}
