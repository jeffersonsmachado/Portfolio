using Moq;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Services;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Tests;

public class UserTests
{
	[Fact]
	public void Create_WithValidData_ShouldReturnUser()
	{
		// Arrange
		string name = "JohnDoe";
		string email = "john@example.com";
		PasswordHash password = PasswordHash.Create("hashedpassword");

		// Act
		var user = User.Create(name, email, password, string.Empty).Value!;

		// Assert
		Assert.NotNull(user);
		Assert.Equal(name, user.Name);
		Assert.Equal(email, user.Email);
		Assert.NotEqual(Guid.Empty, user.Id);
	}

	[Fact]
	public void Recreate_WithValidData_ShouldReturnUserWithSameId()
	{
		// Arrange
		Guid existingId = Guid.NewGuid();
		string name = "JohnDoe";
		string email = "john@example.com";
		PasswordHash password = PasswordHash.Create("hashedpassword");

		// Act
		var user = User.Recreate(existingId, name, email, password, DateTime.UtcNow, false, null).Value!;

		// Assert
		Assert.NotNull(user);
		Assert.Equal(existingId, user.Id);
		Assert.Equal(name, user.Name.Value);
		Assert.Equal(email, user.Email.Value);
	}

	[Fact]
	public void Recreate_WithEmptyGuid_ShouldReturnFailure()
	{
		// Arrange
		Guid emptyId = Guid.Empty;
		PasswordHash password = PasswordHash.Create("hashedpassword");

		// Act
		var result = User.Recreate(emptyId, "JohnDoe", "john@example.com", password, DateTime.UtcNow.AddDays(-500), true, null);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("INVALID_USER_ID", result.Error!.Code);
	}

	[Fact]
	public void Create_ShouldInitializeEmptyProfilesCollection()
	{
		// Arrange & Act
		var user = User.Create("JohnDoe", "john@example.com", PasswordHash.Create("hashedpassword"), string.Empty).Value!;

		// Assert
		Assert.NotNull(user.Profiles);
		Assert.Empty(user.Profiles);
	}

	[Fact]
	public void VerifyToken_WithValidToken_ShouldMarkEmailAsVerified()
	{
		// Arrange
		var token = "123456";
		var user = User.Create("JohnDoe", "john@example.com", PasswordHash.Create("hash"), token).Value!;

		// Act
		var result = user.VerifyToken(token);

		// Assert
		Assert.True(result);
		Assert.True(user.IsEmailVerified);
		Assert.Null(user.VerificationToken);
		Assert.Null(user.VerificationTokenExpiresAt);
	}

	[Fact]
	public void VerifyToken_WithWrongToken_ShouldReturnFalse()
	{
		// Arrange
		var user = User.Create("JohnDoe", "john@example.com", PasswordHash.Create("hash"), "123456").Value!;

		// Act
		var result = user.VerifyToken("wrong_token");

		// Assert
		Assert.False(result);
		Assert.False(user.IsEmailVerified);
	}

	[Fact]
	public void VerifyToken_WithExpiredToken_ShouldReturnFalse()
	{
		// Arrange
		var user = User.Recreate(
			Guid.NewGuid(),
			"JohnDoe",
			"john@example.com",
			PasswordHash.Create("hash"),
			DateTime.UtcNow.AddDays(-2),
			isEmailVerified: false,
			verificationToken: "123456",
			verificationTokenExpiresAt: DateTime.UtcNow.AddHours(-1)
		).Value!;

		// Act
		var result = user.VerifyToken("123456");


		// Assert
		Assert.False(result);
		Assert.False(user.IsEmailVerified);
	}

	[Fact]
	public void VerifyToken_WhenAlreadyVerified_ShouldReturnTrue()
	{
		// Arrange
		var user = User.Recreate(
			Guid.NewGuid(),
			"JohnDoe",
			"john@example.com",
			PasswordHash.Create("hash"),
			DateTime.UtcNow.AddDays(-1),
			isEmailVerified: true
		).Value!;

		// Act
		var result = user.VerifyToken("any_token");

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
	{
		// Arrange
		var mockHasher = new Mock<IPasswordHasher>();
		mockHasher.Setup(h => h.Verify("correct_password", "hashed_password")).Returns(true);
		var user = User.Create("JohnDoe", "john@example.com", PasswordHash.Create("hashed_password"), string.Empty).Value!;

		// Act
		var result = user.VerifyPassword("correct_password", mockHasher.Object);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
	{
		// Arrange
		var mockHasher = new Mock<IPasswordHasher>();
		mockHasher.Setup(h => h.Verify("wrong_password", "hashed_password")).Returns(false);
		var user = User.Create("JohnDoe", "john@example.com", PasswordHash.Create("hashed_password"), string.Empty).Value!;

		// Act
		var result = user.VerifyPassword("wrong_password", mockHasher.Object);

		// Assert
		Assert.False(result);
	}
}
