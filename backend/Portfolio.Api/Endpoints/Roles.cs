using Portfolio.Api.Extensions;
using Portfolio.Api.Filters;
using Portfolio.Application.Users;
using Portfolio.Application.Users.Requests;
using Portfolio.Domain.Shared;

namespace Portfolio.Api.Endpoints;

public static class RolesEndpoints
{
	public static void MapRolesEndpoints(this IEndpointRouteBuilder app, CancellationToken cancellationToken = default)
	{
		var roleGroup = app.MapGroup("/roles").WithOpenApi();

		roleGroup.MapGet("/", async (IRoleService service) =>
		{
			return (await service.GetAllRolesAsync(cancellationToken))
				.ToHttpResult(roles => Results.Ok(roles), failureStatusCode: StatusCodes.Status204NoContent);
		}).RequireAuthorization(Permissions.RolesView);

		roleGroup.MapPost("/", async (CreateRoleRequest req, IRoleService service) =>
		{
			return (await service.CreateRoleAsync(req.Name, cancellationToken))
				.ToHttpResult(role => Results.Created($"/roles/{role.Id}", role));
		}).AddEndpointFilter<ValidationFilter<CreateRoleRequest>>()
		.RequireAuthorization(Permissions.RolesCreate);

		roleGroup.MapPost("/{roleId:guid}/assign/{userId:guid}", async (Guid roleId, Guid userId, IRoleService service) =>
		{
			return (await service.AssignRoleToUserAsync(userId, roleId, cancellationToken))
				.ToHttpResult(_ => Results.Ok(), StatusCodes.Status400BadRequest);
		}).RequireAuthorization(Permissions.RolesUpdate)
		.RequireAuthorization(Permissions.UserUpdate);

		roleGroup.MapDelete("/{id:guid}", async (Guid id, IRoleService service) =>
		{
			return (await service.DeleteRoleAsync(id, cancellationToken))
				.ToHttpResult(_ => Results.Ok(), StatusCodes.Status404NotFound);
		}).RequireAuthorization(Permissions.RolesDelete);

		roleGroup.MapGet("/{id:guid}", async (Guid id, IRoleService service) =>
		{
			return (await service.GetRoleByIdAsync(id, cancellationToken))
				.ToHttpResult(role => Results.Ok(role), StatusCodes.Status404NotFound);
		}).RequireAuthorization(Permissions.RolesView);

		roleGroup.MapGet("/name/{name}", async (string name, IRoleService service) =>
		{
			return (await service.GetRoleByNameAsync(name, cancellationToken))
				.ToHttpResult(role => Results.Ok(role), StatusCodes.Status404NotFound);
		}).RequireAuthorization(Permissions.RolesView);
	}
}