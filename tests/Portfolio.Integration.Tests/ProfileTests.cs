using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Portfolio.Application.Profiles;
using Portfolio.Application.Users;
using Portfolio.Integration.Tests.Fixtures;

namespace Portfolio.Integration.Tests;

/// <summary>
/// Integration tests for Profile API endpoints.
/// These tests validate the full request-response pipeline including routing, 
/// serialization, business logic, and data persistence layers.
/// </summary>
/// <remarks>
/// Uses xUnit's IClassFixture pattern to share a single WebApplicationFactory instance
/// across all tests in this class, improving test performance by reducing startup overhead.
/// The CustomFactory provides a configured in-memory test environment with isolated state.
/// </remarks>
public class PlayerTests(CustomFactory factory) : IClassFixture<CustomFactory>
{
	private readonly HttpClient _client = factory.CreateAuthenticatedClient();

	/// <summary>
	/// Verifies the happy path for player creation via POST /players endpoint.
	/// </summary>
	[Fact]
	public async Task CreatePlayer_WithValidData_ReturnsCreatedAndPlayer()
	{

		// Arrange: Create a user first since Player creation requires an associated User
		CreateUserRequest userRequest = new("USER_TEST_01", "USER_TEST_01@example.com", "SECRET_01", "SECRET_01");

		// Act: Execute the HTTP POST request to create a new user
		var userResponse = await _client.PostAsJsonAsync("/users", userRequest);

		// Assert: Ensure user creation was successful before proceeding with player creation
		userResponse.StatusCode.Should().Be(HttpStatusCode.Created);

		// Extract the created user's details for use in player creation
		var createdUser = await userResponse.Content.ReadFromJsonAsync<UserDto>();

		// Assert: Validate that the user was created successfully and has a valid ID
		createdUser.Should().NotBeNull();

		// Arrange: Prepare a valid CreateProfileRequest using the created user's ID
		CreateProfileRequest newProfile = new()
		{
			Name = "PROFILE_TEST_01",
			UserId = createdUser.Id,
		};

		// Act: Execute the HTTP POST request to create a new profile
		var response = await _client.PostAsJsonAsync("/profiles", newProfile, CancellationToken.None);

		// Assert: Verify HTTP semantics - 201 Created indicates successful resource creation
		response.StatusCode.Should().Be(HttpStatusCode.Created);

		// Assert: Validate response payload structure and content
		var createdProfile = await response.Content.ReadFromJsonAsync<ProfileDto>();
		createdProfile.Should().NotBeNull();

		// Assert: Verify ID generation - essential for entity identity in DDD
		createdProfile.Id.Should().NotBeEmpty();

		// Assert: Confirm request data integrity through round-trip validation
		createdProfile.Name.Should().Be(newProfile.Name);
		createdProfile.UserName.Should().Be(createdUser.Username);
	}

	/// <summary>
	/// Validates profile retrieval by ID through GET /profiles/{id} endpoint.
	/// </summary>
	[Fact]
	public async Task GetProfile_ExistingId_ReturnsOk()
	{
		// Arrange: Create a user and profile to ensure there is an existing profile to retrieve
		CreateUserRequest userRequest = new()
		{
			Username = "USER_TEST_02",
			Email = "USER_TEST_02@example.com",
			Password = "SECRET_02",
			ConfirmPassword = "SECRET_02"
		};
		// Act: Create the user first
		var userResponse = await _client.PostAsJsonAsync("/users", userRequest);

		// Assert: Ensure user creation was successful before proceeding with profile creation
		userResponse.StatusCode.Should().Be(HttpStatusCode.Created);

		// Extract the created user's details for use in profile creation
		var createdUser = await userResponse.Content.ReadFromJsonAsync<UserDto>();

		// Assert: Validate that the user was created successfully and has a valid ID
		createdUser.Should().NotBeNull();

		// Arrange: Prepare a valid CreateProfileRequest using the created user's ID
		CreateProfileRequest newProfile = new()
		{
			Name = "PROFILE_TEST_02",
			UserId = createdUser!.Id
		};

		// Act: Create the profile
		var createResponse = await _client.PostAsJsonAsync("/profiles", newProfile, CancellationToken.None);

		// Assert: Verify profile creation was successful
		createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

		// Assert: Extract the created profile's details for retrieval test
		var createdProfile = await createResponse.Content.ReadFromJsonAsync<ProfileDto>();

		// Assert: Validate that the profile was created successfully and has a valid ID
		createdProfile.Should().NotBeNull();

		// Act: Retrieve the profile by ID
		var getResponse = await _client.GetAsync($"/profiles/{createdProfile!.Id}");

		// Assert: Verify HTTP semantics - 200 OK indicates successful retrieval
		getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
	}
}
