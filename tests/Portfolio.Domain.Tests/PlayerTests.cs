using Portfolio.Domain.Aggregates.Profiles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Tests;

public class PlayerTests
{
	[Fact]
	public void Create_WithValidData_ShouldReturnProfile()
	{
		// Arrange
		string expectedPlayerUserName = "John Doe";

		// Act
		var player = Profile.Create(expectedPlayerUserName, NewDummyUser());

		// Assert
		Assert.NotNull(player);
		Assert.Equal(expectedPlayerUserName, player.Name);
		Assert.NotEqual(Guid.Empty, player.Id);
	}

	[Fact]
	public void Create_WithEmptyUserName_ShouldThrowArgumentException()
	{
		// Arrange
		string invalidUserName = "";

		// Act + Assert
		Assert.Throws<ArgumentException>(() => Profile.Create(invalidUserName, NewDummyUser()));
	}

	[Fact]
	public void Recreate_WithValidData_ShouldReturnProfileWithSameId()
	{
		// Arrange
		Guid existingId = Guid.NewGuid();
		string playerName = "RecreatedPlayer";

		// Act
		var player = Profile.Recreate(existingId, playerName, NewDummyUser());

		// Assert
		Assert.NotNull(player);
		Assert.Equal(existingId, player.Id);
		Assert.Equal(playerName, player.Name);
	}

	[Fact]
	public void Recreate_WithEmptyGuid_ShouldThrowArgumentException()
	{
		// Arrange
		Guid emptyId = Guid.Empty;

		// Act + Assert
		Assert.Throws<ArgumentException>(() => Profile.Recreate(emptyId, "SomeName", NewDummyUser()));
	}

	/**
	 * Static method for creating a dummy User for testing purposes.
	 * This allows us to easily create valid User instances without
	 * needing to set up a full UserService or repository.
	 */
	private static User NewDummyUser(string name = "John Doe", string email = "johndoe@example.com", string password = "password")
	{
		return User.Create(
			name,
			email,
			PasswordHash.Create(password),
			string.Empty
		).Value!;
	}
}