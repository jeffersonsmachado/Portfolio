using Moq;
using Portfolio.Application.Services;
using Portfolio.Application.Users;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Services;
using Portfolio.Domain.Shared;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.Tests;

public class UserServiceTests
{
	[Fact]
	public async Task CreateAsync_NewUser_ShouldReturnUserDto()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetByNameAsync("JohnDoe", It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		mockHasher.Setup(h => h.Hash(It.IsAny<string>()))
			.Returns("hashed_password");

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		CreateUserRequest request = new("JohnDoe", "john@example.com", "password123", "password123");

		// Act
		Result<UserDto> result = await userService.CreateAsync(request);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("JohnDoe", result.Value.Username);
		Assert.Equal("john@example.com", result.Value.Email);
		mockUserRepo.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
		mockEmailService.Verify(svc => svc.SendEmailAsync(
			"john@example.com",
			It.IsAny<string>(),
			It.IsAny<string>(),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task CreateAsync_ExistingUsername_ShouldReturnFailureResult()
	{
		// Arrange
		User existingUser = NewDummyUser("JohnDoe").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetByNameAsync("JohnDoe", It.IsAny<CancellationToken>()))
			.ReturnsAsync(existingUser);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		CreateUserRequest request = new("JohnDoe", "john@example.com", "password123", "password123");

		// Act
		Result<UserDto> result = await userService.CreateAsync(request);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("USERNAME_ALREADY_EXISTS", result.Error!.Code);
		mockUserRepo.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task GetByIdAsync_ExistingUser_ShouldReturnUserDto()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		// Act
		Result<UserDto> result = await userService.GetByIdAsync(user.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("JohnDoe", result.Value.Username);
	}

	[Fact]
	public async Task GetByIdAsync_NonExistingUser_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		// Act
		Result<UserDto> result = await userService.GetByIdAsync(Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task GetByNameAsync_ExistingUser_ShouldReturnUserDto()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetByNameAsync("JohnDoe", It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		// Act
		Result<UserDto> result = await userService.GetByNameAsync("JohnDoe");

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("JohnDoe", result.Value.Username);
	}

	[Fact]
	public async Task GetByNameAsync_NonExistingUser_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetByNameAsync("Ghost", It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		// Act
		Result<UserDto> result = await userService.GetByNameAsync("Ghost");

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task GetAllAsync_WithUsers_ShouldReturnUserDtos()
	{
		// Arrange		
		var users = new List<User>
		{
			NewDummyUser("UserOne", "one@example.com").Value!,
			NewDummyUser("UserTwo", "two@example.com").Value!
		};

		if (users.Any(u => u == null))
			Assert.Fail("User creation failed for one of the users.");


		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		// Act
		Result<IEnumerable<UserDto>> result = await userService.GetAllAsync();

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal(2, result.Value.Count());
	}

	[Fact]
	public async Task GetAllAsync_NoUsers_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync([]);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		// Act
		Result<IEnumerable<UserDto>> result = await userService.GetAllAsync();

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("NO_USERS_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task DeleteAsync_ExistingUser_ShouldReturnUserDto()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		// Act
		Result<UserDto> result = await userService.DeleteAsync(user.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("JohnDoe", result.Value.Username);
		mockUserRepo.Verify(repo => repo.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_NonExistingUser_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IPasswordHasher> mockHasher = new();
		Mock<IEmailService> mockEmailService = new();

		mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		UserService userService = new(mockUserRepo.Object, mockHasher.Object, mockEmailService.Object);

		// Act
		Result<UserDto> result = await userService.DeleteAsync(Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Null(result.Value);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
		mockUserRepo.Verify(repo => repo.DeleteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	private static Result<User> NewDummyUser(string name = "JohnDoe", string email = "johndoe@example.com", string password = "password")
	{
		return User.Create(
			name,
			email,
			PasswordHash.Create(password),
			string.Empty
		);
	}
}
