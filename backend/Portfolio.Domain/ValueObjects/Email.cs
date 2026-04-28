namespace Portfolio.Domain.ValueObjects;

using System.Text.RegularExpressions;

public record Email
{
	public string Value { get; init; }

	private Email() => Value = null!;
	private Email(string value) => Value = value;

	public static Email Parse(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			throw new ArgumentException("Email cannot be null or empty", nameof(email));
		}

		var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
		if (!Regex.IsMatch(email, emailPattern))
		{
			throw new ArgumentException("Invalid email format", nameof(email));
		}

		return new Email(email);
	}

	public static Email Create(string email)
	{
		return new Email(email);
	}

	public static implicit operator string(Email email) => email.Value;

}