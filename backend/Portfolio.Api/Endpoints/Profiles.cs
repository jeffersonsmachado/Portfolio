using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Portfolio.Api.Extensions;
using Portfolio.Api.Filters;
using Portfolio.Application.Profiles;

namespace Portfolio.Api.Endpoints;

public static class ProfilesEndpoints
{
	public static void MapProfilesEndpoints(this IEndpointRouteBuilder app, CancellationToken cancellationToken = default)
	{
		var profileGroup = app.MapGroup("/profiles").WithOpenApi();

		profileGroup.RequireAuthorization();

		profileGroup.MapPost("/", async (CreateProfileRequest req, IProfileService service) =>
		{
			return (await service.CreateAsync(req))
				.ToHttpResult(profile => Results.Created($"/profiles/{profile.Id}", profile), failureStatusCode: StatusCodes.Status400BadRequest);
		}).AddEndpointFilter<ValidationFilter<CreateProfileRequest>>();

		profileGroup.MapGet("/{id:guid}", async (Guid id, IProfileService service) =>
		{
			return (await service.GetByIdAsync(id))
				.ToHttpResult(profile => Results.Ok(profile), StatusCodes.Status404NotFound);
		});

		profileGroup.MapGet("/", async (IProfileService service) =>
		{
			return (await service.GetAllAsync())
				.ToHttpResult(profiles => Results.Ok(profiles), failureStatusCode: StatusCodes.Status204NoContent);
		});

		profileGroup.MapDelete("/{id:guid}", async (Guid id, IProfileService service) =>
		{
			return (await service.DeleteAsync(id))
				.ToHttpResult(profile => Results.Ok(profile), failureStatusCode: StatusCodes.Status404NotFound);

		});

		profileGroup.MapGet("/me", async (ClaimsPrincipal user, IProfileService service) =>
		{
			// get ID from claims
			var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub);
			if (userIdClaim == null)
			{
				return Results.Unauthorized();
			}

			if (!Guid.TryParse(userIdClaim.Value, out var userId))
			{
				return Results.Unauthorized();
			}

			var profileResult = await service.GetProfileByUserIdAsync(userId, cancellationToken);
			if (!profileResult.IsSuccess)
			{
				return Results.NotFound();
			}

			return Results.Ok(profileResult.Value);
		});
	}
}