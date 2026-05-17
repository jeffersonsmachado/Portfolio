using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Portfolio.Api.Extensions;
using Portfolio.Api.Filters;
using Portfolio.Application.Profiles;

namespace Portfolio.Api.Endpoints;

public static class ProfilesEndpoints
{
	static Guid? GetUserId(ClaimsPrincipal user)
	{
		var claim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub);
		return Guid.TryParse(claim?.Value, out var id) ? id : null;
	}
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
			var userId = GetUserId(user);
			if (userId == null)
			{
				return Results.Unauthorized();
			}

			var profileResult = await service.GetProfileByUserIdAsync(userId.Value, cancellationToken);
			if (!profileResult.IsSuccess)
			{
				return Results.NotFound();
			}

			return Results.Ok(profileResult.Value);
		});

		// PUT /profiles/me
		profileGroup.MapPut("/me", async (ClaimsPrincipal user, UpdateProfileRequest req, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.UpdateAsync(profileResult.Value!.Id, req))
				.ToHttpResult(p => Results.Ok(p), StatusCodes.Status400BadRequest);
		});

		// POST /profiles/me/skills
		profileGroup.MapPost("/me/skills", async (ClaimsPrincipal user, AddSkillRequest req, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.AddSkillAsync(profileResult.Value!.Id, req))
				.ToHttpResult(s => Results.Created($"/profiles/me/skills/{s.Id}", s), StatusCodes.Status400BadRequest);
		});

		// DELETE /profiles/me/skills/{skillId}
		profileGroup.MapDelete("/me/skills/{skillId:guid}", async (ClaimsPrincipal user, Guid skillId, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.RemoveSkillAsync(profileResult.Value!.Id, skillId))
				.ToHttpResult(_ => Results.NoContent(), StatusCodes.Status404NotFound);
		});

		// POST /profiles/me/experiences
		profileGroup.MapPost("/me/experiences", async (ClaimsPrincipal user, AddExperienceRequest req, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.AddExperienceAsync(profileResult.Value!.Id, req))
				.ToHttpResult(e => Results.Created($"/profiles/me/experiences/{e.Id}", e), StatusCodes.Status400BadRequest);
		});

		// PUT /profiles/me/experiences/{experienceId}
		profileGroup.MapPut("/me/experiences/{experienceId:guid}", async (ClaimsPrincipal user, Guid experienceId, UpdateExperienceRequest req, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.UpdateExperienceAsync(profileResult.Value!.Id, experienceId, req))
				.ToHttpResult(e => Results.Ok(e), StatusCodes.Status404NotFound);
		});

		// DELETE /profiles/me/experiences/{experienceId}
		profileGroup.MapDelete("/me/experiences/{experienceId:guid}", async (ClaimsPrincipal user, Guid experienceId, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.RemoveExperienceAsync(profileResult.Value!.Id, experienceId))
				.ToHttpResult(_ => Results.NoContent(), StatusCodes.Status404NotFound);
		});

		// POST /profiles/me/educations
		profileGroup.MapPost("/me/educations", async (ClaimsPrincipal user, AddEducationRequest req, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.AddEducationAsync(profileResult.Value!.Id, req))
				.ToHttpResult(e => Results.Created($"/profiles/me/educations/{e.Id}", e), StatusCodes.Status400BadRequest);
		});

		// PUT /profiles/me/educations/{educationId}
		profileGroup.MapPut("/me/educations/{educationId:guid}", async (ClaimsPrincipal user, Guid educationId, UpdateEducationRequest req, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.UpdateEducationAsync(profileResult.Value!.Id, educationId, req))
				.ToHttpResult(e => Results.Ok(e), StatusCodes.Status404NotFound);
		});

		// DELETE /profiles/me/educations/{educationId}
		profileGroup.MapDelete("/me/educations/{educationId:guid}", async (ClaimsPrincipal user, Guid educationId, IProfileService service) =>
		{
			var profileResult = await service.GetProfileByUserIdAsync(GetUserId(user) ?? Guid.Empty);
			if (!profileResult.IsSuccess) return Results.NotFound();
			return (await service.RemoveEducationAsync(profileResult.Value!.Id, educationId))
				.ToHttpResult(_ => Results.NoContent(), StatusCodes.Status404NotFound);
		});

	}
}