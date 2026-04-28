using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Users;
using Portfolio.Application.Validations;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Integration.Tests.Fixtures;

namespace Portfolio.Integration.Tests;

/// <summary>
/// Integration tests for User API endpoints.
/// These tests validate the full request-response pipeline for user registration,
/// email verification, and login flows.
/// </summary>
public class UserTests(CustomFactory factory) : IClassFixture<CustomFactory>
{
	private readonly HttpClient _client = factory.CreateClient();
	private readonly CustomFactory _factory = factory;

	/// <summary>
	/// Verifies that a new user can be created successfully via POST /users.
	/// </summary>
	[Fact]
	public async Task CreateUser_WithValidData_ReturnsCreated()
	{
		// Arrange
		var request = new CreateUserRequest("USER_IT_01", "user_it_01@example.com", "Password123!", "Password123!");

		// Act
		var response = await _client.PostAsJsonAsync("/users", request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Created);
		var created = await response.Content.ReadFromJsonAsync<UserDto>();
		created.Should().NotBeNull();
		created!.Username.Should().Be("USER_IT_01");
		created.Email.Should().Be("user_it_01@example.com");
		created.Id.Should().NotBeEmpty();
	}

	/// <summary>
	/// Verifies that creating a user with a duplicate username returns a failure response.
	/// </summary>
	[Fact]
	public async Task CreateUser_WithExistingUsername_ReturnsBadRequest()
	{
		// Arrange
		var request = new CreateUserRequest("USER_IT_DUP", "dup@example.com", "Password123!", "Password123!");
		await _client.PostAsJsonAsync("/users", request);

		// Act: second registration with same username
		var response = await _client.PostAsJsonAsync("/users", request);

		// Assert
		((int)response.StatusCode).Should().BeGreaterThanOrEqualTo(400);
	}

	/// <summary>
	/// Verifies that a user can verify their email with a valid token.
	/// </summary>
	[Fact]
	public async Task VerifyToken_WithValidToken_ReturnsOk()
	{
		// Arrange: create user
		var createRequest = new CreateUserRequest("USER_IT_VT", "user_it_vt@example.com", "Password123!", "Password123!");
		var createResponse = await _client.PostAsJsonAsync("/users", createRequest);
		createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

		// Read the verification token directly from the DB (bypasses email delivery)
		var token = await GetVerificationTokenAsync("user_it_vt@example.com");
		token.Should().NotBeNullOrEmpty();

		// Act
		var verifyRequest = new VerifyTokenRequest("user_it_vt@example.com", token!);
		var verifyResponse = await _client.PostAsJsonAsync("/users/verify", verifyRequest);

		// Assert
		verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	/// <summary>
	/// Verifies that an invalid verification token returns a bad request.
	/// </summary>
	[Fact]
	public async Task VerifyToken_WithInvalidToken_ReturnsBadRequest()
	{
		// Arrange
		var createRequest = new CreateUserRequest("USER_IT_IVT", "user_it_ivt@example.com", "Password123!", "Password123!");
		await _client.PostAsJsonAsync("/users", createRequest);

		// Act
		var verifyRequest = new VerifyTokenRequest("user_it_ivt@example.com", "000000");
		var verifyResponse = await _client.PostAsJsonAsync("/users/verify", verifyRequest);

		// Assert
		verifyResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	/// <summary>
	/// Verifies that login fails when the email has not been verified.
	/// </summary>
	[Fact]
	public async Task Login_WithUnverifiedEmail_ReturnsUnauthorized()
	{
		// Arrange: create user but do NOT verify
		var createRequest = new CreateUserRequest("USER_IT_UNV", "user_it_unv@example.com", "Password123!", "Password123!");
		await _client.PostAsJsonAsync("/users", createRequest);

		// Act
		var loginRequest = new LoginRequest("user_it_unv@example.com", "Password123!");
		var loginResponse = await _client.PostAsJsonAsync("/users/login", loginRequest);

		// Assert
		loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	/// <summary>
	/// Verifies that a verified user can login and receives a JWT token.
	/// </summary>
	[Fact]
	public async Task Login_WithValidCredentials_ReturnsToken()
	{
		// Arrange: create and verify user
		var createRequest = new CreateUserRequest("USER_IT_LOG", "user_it_log@example.com", "Password123!", "Password123!");
		await _client.PostAsJsonAsync("/users", createRequest);

		var token = await GetVerificationTokenAsync("user_it_log@example.com");
		await _client.PostAsJsonAsync("/users/verify", new VerifyTokenRequest("user_it_log@example.com", token!));

		// Act
		var loginRequest = new LoginRequest("user_it_log@example.com", "Password123!");
		var loginResponse = await _client.PostAsJsonAsync("/users/login", loginRequest);

		// Assert
		loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
		var body = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
		body.Should().NotBeNull();
		body!.Token.Should().NotBeNullOrEmpty();
	}

	/// <summary>
	/// Verifies that login fails when the password is incorrect.
	/// </summary>
	[Fact]
	public async Task Login_WithWrongPassword_ReturnsUnauthorized()
	{
		// Arrange: create and verify user
		var createRequest = new CreateUserRequest("USER_IT_WP", "user_it_wp@example.com", "Password123!", "Password123!");
		await _client.PostAsJsonAsync("/users", createRequest);

		var token = await GetVerificationTokenAsync("user_it_wp@example.com");
		await _client.PostAsJsonAsync("/users/verify", new VerifyTokenRequest("user_it_wp@example.com", token!));

		// Act
		var loginRequest = new LoginRequest("user_it_wp@example.com", "WrongPassword!");
		var loginResponse = await _client.PostAsJsonAsync("/users/login", loginRequest);

		// Assert
		loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	/// <summary>
	/// Reads the verification token for a given email directly from the in-memory database.
	/// This simulates what the user would receive via email in production.
	/// </summary>
	private async Task<string?> GetVerificationTokenAsync(string email)
	{
		using var scope = _factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
		var users = await db.Users.ToListAsync();
		return users.FirstOrDefault(u => u.Email.Value == email)?.VerificationToken;
	}

	private record ResourceCapabilities(bool CanCreate, bool CanRead, bool CanUpdate, bool CanDelete);
	private record TokenResponse(string Token, Dictionary<string, ResourceCapabilities> Capabilities);
}
