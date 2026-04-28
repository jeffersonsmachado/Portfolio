using Portfolio.Domain.Services;

namespace Portfolio.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
	public string Hash(string rawPassword)
	{
		// Implement a simple hash for demonstration purposes
		// In production, use a secure hashing algorithm like BCrypt or Argon2
		var bytes = System.Text.Encoding.UTF8.GetBytes(rawPassword);
		var hashBytes = System.Security.Cryptography.SHA256.HashData(bytes);
		return Convert.ToBase64String(hashBytes);
	}

	public bool Verify(string rawPassword, string hashedPassword)
	{
		var hashOfInput = Hash(rawPassword);
		return hashOfInput == hashedPassword;
	}
}