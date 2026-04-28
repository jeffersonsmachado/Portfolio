using Moq;
using Portfolio.Application.Services;
using Portfolio.Application.Users;
using Portfolio.Application.Validations;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Services;
using Portfolio.Domain.Shared;
using Portfolio.Domain.ValueObjects;
using Portfolio.Application.Users.Dtos;

namespace Portfolio.Application.Tests;

public class AuthServiceTests
{
	[Fact]
	public async Task VerifyToken_WithValidToken_ShouldReturnUserDto()
	{
		// Arrange
		string token = "123456";
		User user = User.Create("JohnDoe", "john@example.com", PasswordHash.Create("hash"), token).Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<UserDto> result = await authService.VerifyToken(new VerifyTokenRequest("john@example.com", token));

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("JohnDoe", result.Value.Username);
		mockUserRepo.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task VerifyToken_WithNonExistingEmail_ShouldReturnFailureResult()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<UserDto> result = await authService.VerifyToken(new VerifyTokenRequest("nonexistent@example.com", "123456"));

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
		mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task VerifyToken_WithInvalidToken_ShouldReturnFailureResult()
	{
		// Arrange
		User user = User.Create("JohnDoe", "john@example.com", PasswordHash.Create("hash"), "123456").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<UserDto> result = await authService.VerifyToken(new VerifyTokenRequest("john@example.com", "wrong_token"));

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("INVALID_TOKEN", result.Error!.Code);
		mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task LoginAsync_WithNonExistingEmail_ShouldReturnUserNotFound()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<LoginDto> result = await authService.LoginAsync(new LoginRequest("nobody@example.com", "password"));

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
		mockJwtProvider.Verify(jp => jp.Generate(It.IsAny<User>()), Times.Never);
	}

	[Fact]
	public async Task LoginAsync_WithUnverifiedEmail_ShouldReturnEmailNotVerified()
	{
		// Arrange
		User unverifiedUser = NewDummyUser("JohnDoe", "john@example.com").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
			.ReturnsAsync(unverifiedUser);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<LoginDto> result = await authService.LoginAsync(new LoginRequest("john@example.com", "password"));

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("EMAIL_NOT_VERIFIED", result.Error!.Code);
		mockJwtProvider.Verify(jp => jp.Generate(It.IsAny<User>()), Times.Never);
	}

	[Fact]
	public async Task LoginAsync_WithInvalidPassword_ShouldReturnInvalidCredentials()
	{
		// Arrange
		User verifiedUser = NewVerifiedUser("JohnDoe", "john@example.com", "hashed_password").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
			.ReturnsAsync(verifiedUser);

		mockHasher.Setup(h => h.Verify("wrong_password", "hashed_password"))
			.Returns(false);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<LoginDto> result = await authService.LoginAsync(new LoginRequest("john@example.com", "wrong_password"));

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("INVALID_CREDENTIALS", result.Error!.Code);
		mockJwtProvider.Verify(jp => jp.Generate(It.IsAny<User>()), Times.Never);
	}

	[Fact]
	public async Task LoginAsync_WithValidCredentials_ShouldReturnTokenAndCapabilities()
	{
		// Arrange
		User verifiedUser = NewVerifiedUser("JohnDoe", "john@example.com", "hashed_password").Value!;
		var role = Role.Create("Admin", [Permission.Create("profile:view"), Permission.Create("profile:create")], []);
		verifiedUser.AssignRole(role);

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
			.ReturnsAsync(verifiedUser);

		mockHasher.Setup(h => h.Verify("correct_password", "hashed_password"))
			.Returns(true);

		mockJwtProvider.Setup(jp => jp.Generate(verifiedUser))
			.Returns("jwt_token_value");

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<LoginDto> result = await authService.LoginAsync(new LoginRequest("john@example.com", "correct_password"));

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal("jwt_token_value", result.Value!.Token);
		Assert.True(result.Value.Capabilities.TryGetValue("profile", out var profileCaps));
		Assert.True(profileCaps!.CanRead);
		Assert.True(profileCaps.CanCreate);
		Assert.False(profileCaps.CanDelete);
		mockJwtProvider.Verify(jp => jp.Generate(verifiedUser), Times.Once);
	}

	[Fact]
	public async Task ResendVerificationTokenAsync_WithNonExistingEmail_ShouldReturnUserNotFound()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<string> result = await authService.ResendVerificationTokenAsync(new ResendVerificationTokenRequest("nobody@example.com"));

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
		mockEmailService.Verify(svc => svc.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task ResendVerificationTokenAsync_WithAlreadyVerifiedEmail_ShouldReturnEmailAlreadyVerified()
	{
		// Arrange
		User verifiedUser = NewVerifiedUser("JohnDoe", "john@example.com").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
			.ReturnsAsync(verifiedUser);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<string> result = await authService.ResendVerificationTokenAsync(new ResendVerificationTokenRequest("john@example.com"));

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("EMAIL_ALREADY_VERIFIED", result.Error!.Code);
		mockEmailService.Verify(svc => svc.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task ResendVerificationTokenAsync_WithUnverifiedEmail_ShouldSendEmailAndReturnSuccess()
	{
		// Arrange
		User unverifiedUser = NewDummyUser("JohnDoe", "john@example.com").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IEmailService> mockEmailService = new();
		Mock<IJwtProvider> mockJwtProvider = new();
		Mock<IPasswordHasher> mockHasher = new();

		mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
			.ReturnsAsync(unverifiedUser);

		AuthService authService = new(mockUserRepo.Object, mockEmailService.Object, mockJwtProvider.Object, mockHasher.Object);

		// Act
		Result<string> result = await authService.ResendVerificationTokenAsync(new ResendVerificationTokenRequest("john@example.com"));

		// Assert
		Assert.True(result.IsSuccess);
		mockUserRepo.Verify(repo => repo.UpdateAsync(unverifiedUser, It.IsAny<CancellationToken>()), Times.Once);
		mockEmailService.Verify(svc => svc.SendEmailAsync(
			"john@example.com",
			It.IsAny<string>(),
			It.IsAny<string>(),
			It.IsAny<CancellationToken>()), Times.Once);
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

	private static Result<User> NewVerifiedUser(string name = "JohnDoe", string email = "johndoe@example.com", string password = "hashed_password")
	{
		return User.Recreate(
			Guid.NewGuid(),
			name,
			email,
			PasswordHash.Create(password),
			DateTime.UtcNow.AddDays(-1),
			isEmailVerified: true
		);
	}
}
