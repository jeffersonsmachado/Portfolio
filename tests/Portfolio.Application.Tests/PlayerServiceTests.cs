using Moq;
using Portfolio.Application.Profiles;
using Portfolio.Domain.Aggregates.Profiles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Shared;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.Tests;

public class PlayerServiceTests
{
	[Fact]
	public async Task Create_NewPlayer_OnValidUser_ShouldReturnPlayerDto()
	{
		// Arrange
		User dummy = NewDummyUser("John");

		// "Spy" (The Mock Object)
		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		// STUB: Profile DOES NOT exist
		mockPlayerRepo.Setup(repo => repo.GetByNameAsync("Gretchen", It.IsAny<CancellationToken>()))
			.ReturnsAsync((Profile?)null);

		// STUB: User DOES exist
		mockUserRepo.Setup(repo => repo.GetByIdAsync(dummy.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(dummy);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		CreateProfileRequest playerRequest = new()
		{
			Name = "Player Zero",
			UserId = dummy.Id
		};

		// Act
		Result<ProfileDto> createdPlayer = await playerService.CreateAsync(playerRequest);

		Assert.True(createdPlayer.IsSuccess);
		Assert.NotNull(createdPlayer.Value);
		Assert.Equal("Player Zero", createdPlayer.Value.Name);

		ProfileDto playerDto = createdPlayer.Value;

		// Assert
		Assert.NotNull(playerDto);
		Assert.Equal("Player Zero", playerDto.Name);

		// Mocking
		// Verify that AddAsync was called exactly once
		mockPlayerRepo.Verify(repo => repo.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Create_NewPlayer_OnInvalidUser_ShouldReturnFailureResult()
	{
		// Arrange
		User dummy = NewDummyUser("John");

		// "Spy" (The Mock Object)
		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		// STUB: Profile DOES NOT exist
		mockPlayerRepo.Setup(repo => repo.GetByNameAsync("Gretchen", It.IsAny<CancellationToken>()))
			.ReturnsAsync((Profile?)null);

		// STUB: User DOES NOT exist
		mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		CreateProfileRequest playerRequest = new()
		{
			Name = "Player Zero",
			UserId = Guid.NewGuid() // Random GUID that does not match the dummy user
		};


		// Mocking
		// Verify that AddAsync was never called
		mockPlayerRepo.Verify(repo => repo.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()), Times.Never);

		// Act
		Result<ProfileDto> createdPlayer = await playerService.CreateAsync(playerRequest);
		Assert.False(createdPlayer.IsSuccess);
		Assert.Null(createdPlayer.Value);


	}

	[Fact]
	public async Task Create_WithExistingPlayerName_ShouldReturnFailureResult()
	{
		// Arrange
		User dummy = NewDummyUser();
		Profile existingPlayer = Profile.Create("Player Zero", dummy);

		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		// STUB: Profile ALREADY exists
		mockPlayerRepo.Setup(repo => repo.GetByNameAsync("Player Zero", It.IsAny<CancellationToken>()))
			.ReturnsAsync(existingPlayer);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		CreateProfileRequest playerRequest = new()
		{
			Name = "Player Zero",
			UserId = dummy.Id
		};

		// Act
		Result<ProfileDto> result = await playerService.CreateAsync(playerRequest);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("USERNAME_ALREADY_EXISTS", result.Error!.Code);
		mockPlayerRepo.Verify(repo => repo.AddAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task GetByIdAsync_ExistingPlayer_ShouldReturnPlayerDto()
	{
		// Arrange
		User dummy = NewDummyUser();
		Profile player = Profile.Create("TestPlayer", dummy);

		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		mockPlayerRepo.Setup(repo => repo.GetByIdAsync(player.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(player);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		// Act
		Result<ProfileDto> result = await playerService.GetByIdAsync(player.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("TestPlayer", result.Value.Name);
	}

	[Fact]
	public async Task GetByIdAsync_NonExistingPlayer_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		mockPlayerRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Profile?)null);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		// Act
		Result<ProfileDto> result = await playerService.GetByIdAsync(Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("PROFILE_NOT_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task GetByNameAsync_ExistingPlayer_ShouldReturnPlayerDto()
	{
		// Arrange
		User dummy = NewDummyUser();
		Profile player = Profile.Create("TestPlayer", dummy);

		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		mockPlayerRepo.Setup(repo => repo.GetByNameAsync("TestPlayer", It.IsAny<CancellationToken>()))
			.ReturnsAsync(player);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		// Act
		Result<ProfileDto> result = await playerService.GetByNameAsync("TestPlayer");

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("TestPlayer", result.Value.Name);
	}

	[Fact]
	public async Task GetByNameAsync_NonExistingPlayer_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		mockPlayerRepo.Setup(repo => repo.GetByNameAsync("Ghost", It.IsAny<CancellationToken>()))
			.ReturnsAsync((Profile?)null);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		// Act
		Result<ProfileDto> result = await playerService.GetByNameAsync("Ghost");

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("PROFILE_NOT_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task GetAllAsync_WithPlayers_ShouldReturnPlayerDtos()
	{
		// Arrange
		User dummy = NewDummyUser();
		List<Profile> players =
		[
			Profile.Create("PlayerOne", dummy),
			Profile.Create("PlayerTwo", dummy)
		];

		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		mockPlayerRepo.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(players);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		// Act
		Result<IEnumerable<ProfileDto>> result = await playerService.GetAllAsync();

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal(2, result.Value.Count());
	}

	[Fact]
	public async Task GetAllAsync_NoPlayers_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		mockPlayerRepo.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync([]);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		// Act
		Result<IEnumerable<ProfileDto>> result = await playerService.GetAllAsync();

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("NO_PROFILES_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task DeleteAsync_ExistingPlayer_ShouldReturnPlayerDto()
	{
		// Arrange
		User dummy = NewDummyUser();
		Profile player = Profile.Create("PlayerToDelete", dummy);

		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		mockPlayerRepo.Setup(repo => repo.GetByIdAsync(player.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(player);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		// Act
		Result<ProfileDto> result = await playerService.DeleteAsync(player.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("PlayerToDelete", result.Value.Name);
		mockPlayerRepo.Verify(repo => repo.DeleteAsync(player.Id, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_NonExistingPlayer_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IProfileRepository> mockPlayerRepo = new();
		Mock<IUserRepository> mockUserRepo = new();

		mockPlayerRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Profile?)null);

		ProfileService playerService = new(mockPlayerRepo.Object, mockUserRepo.Object);

		// Act
		Result<ProfileDto> result = await playerService.DeleteAsync(Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("PROFILE_NOT_FOUND", result.Error!.Code);
		mockPlayerRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
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
