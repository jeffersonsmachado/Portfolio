using System.Net;
using FluentAssertions;
using Portfolio.Integration.Tests.Fixtures;

namespace Portfolio.Integration.Tests;

public class ProfileFailureTests(CustomFactory factory) : IClassFixture<CustomFactory>
{
	private readonly HttpClient _client = factory.CreateAuthenticatedClient();

	[Fact]
	public async Task GetProfile_NonExistingId_ReturnsNotFound()
	{
		// Arrange — random ID guaranteed not to exist in the in-memory test DB
		var nonExistingId = Guid.NewGuid();

		// Act
		var response = await _client.GetAsync($"/profiles/{nonExistingId}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}
}
