using Portfolio.Domain.Aggregates.Profiles;

namespace Portfolio.Domain.ValueObjects;

public record UserName
{
	public string Value { get; init; }

	private UserName(string value) => Value = value;

	public static UserName Parse(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Username cannot be null or whitespace", nameof(name));
		}

		if (name.Length < ProfileConstants.MinNameLength || name.Length > ProfileConstants.MaxNameLength)
		{
			throw new ArgumentException($"Username must be between {ProfileConstants.MinNameLength} and {ProfileConstants.MaxNameLength} characters", nameof(name));
		}

		return new UserName(name);
	}

	public static UserName Create(string name)
	{
		return new UserName(name);
	}

	public static implicit operator string(UserName userName) => userName.Value;
}