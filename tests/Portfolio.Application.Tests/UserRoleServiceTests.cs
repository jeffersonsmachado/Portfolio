using Moq;
using Portfolio.Application.Users;
using Portfolio.Domain.Aggregates.Roles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Shared;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.Tests;

public class UserRoleServiceTests
{
	[Fact]
	public async Task AssignRoleAsync_WithValidUserAndRole_ShouldReturnRoleDto()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;
		Role role = Role.Create("Admin");

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);
		mockRoleRepo.Setup(repo => repo.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(role);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<RoleDto> result = await service.AssignRoleAsync(user.Id, role.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal("Admin", result.Value!.Name);
		mockUserRepo.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task AssignRoleAsync_WithNonExistingUser_ShouldReturnUserNotFound()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<RoleDto> result = await service.AssignRoleAsync(Guid.NewGuid(), Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
		mockRoleRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task AssignRoleAsync_WithNonExistingRole_ShouldReturnRoleNotFound()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);
		mockRoleRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Role?)null);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<RoleDto> result = await service.AssignRoleAsync(user.Id, Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("ROLE_NOT_FOUND", result.Error!.Code);
		mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task RemoveRoleAsync_WithValidUserAndRole_ShouldReturnRoleDto()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;
		Role role = Role.Create("Admin");
		user.AssignRole(role);

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);
		mockRoleRepo.Setup(repo => repo.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(role);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<RoleDto> result = await service.RemoveRoleAsync(user.Id, role.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal("Admin", result.Value!.Name);
		mockUserRepo.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task RemoveRoleAsync_WithNonExistingUser_ShouldReturnUserNotFound()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<RoleDto> result = await service.RemoveRoleAsync(Guid.NewGuid(), Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task RemoveRoleAsync_WithNonExistingRole_ShouldReturnRoleNotFound()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);
		mockRoleRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Role?)null);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<RoleDto> result = await service.RemoveRoleAsync(user.Id, Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("ROLE_NOT_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task GetUserRolesAsync_WithRolesAssigned_ShouldReturnRoleDtos()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;
		user.AssignRole(Role.Create("Admin"));
		user.AssignRole(Role.Create("User"));

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<IEnumerable<RoleDto>> result = await service.GetUserRolesAsync(user.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(2, result.Value!.Count());
	}

	[Fact]
	public async Task GetUserRolesAsync_WithNonExistingUser_ShouldReturnUserNotFound()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<IEnumerable<RoleDto>> result = await service.GetUserRolesAsync(Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
	}

	[Fact]
	public async Task GetUsersInRoleAsync_WithExistingRole_ShouldReturnUserDtos()
	{
		// Arrange
		Role role = Role.Create("Admin");
		var usersInRole = new List<User>
		{
			NewDummyUser("UserOne", "one@example.com").Value!,
			NewDummyUser("UserTwo", "two@example.com").Value!
		};

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockRoleRepo.Setup(repo => repo.GetByIdAsync(role.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(role);
		mockUserRepo.Setup(repo => repo.GetUsersInRoleAsync(role.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(usersInRole);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<IEnumerable<UserDto>> result = await service.GetUsersInRoleAsync(role.Id);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(2, result.Value!.Count());
	}

	[Fact]
	public async Task GetUsersInRoleAsync_WithNonExistingRole_ShouldReturnRoleNotFound()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockRoleRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Role?)null);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<IEnumerable<UserDto>> result = await service.GetUsersInRoleAsync(Guid.NewGuid());

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("ROLE_NOT_FOUND", result.Error!.Code);
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

	[Fact]
	public async Task SetUserRolesAsync_ShouldAddAndRemoveRoles()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;
		Role roleAdmin = Role.Create("Admin");
		Role roleUser = Role.Create("User");
		user.AssignRole(roleAdmin); // starts with Admin

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);
		mockRoleRepo.Setup(repo => repo.GetByIdAsync(roleUser.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(roleUser);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act: set roles to [User] only — Admin should be removed, User added
		Result<IEnumerable<RoleDto>> result = await service.SetUserRolesAsync(user.Id, [roleUser.Id]);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Single(result.Value!);
		Assert.Equal("User", result.Value!.First().Name);
		mockUserRepo.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task SetUserRolesAsync_WithNonExistingUser_ShouldReturnUserNotFound()
	{
		// Arrange
		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<IEnumerable<RoleDto>> result = await service.SetUserRolesAsync(Guid.NewGuid(), [Guid.NewGuid()]);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("USER_NOT_FOUND", result.Error!.Code);
		mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task SetUserRolesAsync_WithNonExistingRole_ShouldReturnRoleNotFound()
	{
		// Arrange
		User user = NewDummyUser("JohnDoe").Value!;

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetByIdWithRolesAsync(user.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);
		mockRoleRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Role?)null);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<IEnumerable<RoleDto>> result = await service.SetUserRolesAsync(user.Id, [Guid.NewGuid()]);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("ROLE_NOT_FOUND", result.Error!.Code);
		mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task GetAllUsersWithRolesAsync_ShouldReturnUsersWithTheirRoles()
	{
		// Arrange
		User user1 = NewDummyUser("Alice", "alice@example.com").Value!;
		User user2 = NewDummyUser("Bob", "bob@example.com").Value!;
		user1.AssignRole(Role.Create("Admin"));
		user2.AssignRole(Role.Create("User"));

		Mock<IUserRepository> mockUserRepo = new();
		Mock<IRoleRepository> mockRoleRepo = new();

		mockUserRepo.Setup(repo => repo.GetAllWithRolesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync([user1, user2]);

		UserRoleService service = new(mockUserRepo.Object, mockRoleRepo.Object);

		// Act
		Result<IEnumerable<UserWithRolesDto>> result = await service.GetAllUsersWithRolesAsync();

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(2, result.Value!.Count());
		Assert.Equal("Alice", result.Value!.First().Username);
		Assert.Single(result.Value!.First().Roles);
		Assert.Equal("Admin", result.Value!.First().Roles.First().Name);
	}
}
