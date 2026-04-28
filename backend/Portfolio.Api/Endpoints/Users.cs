using Portfolio.Api.Extensions;
using Portfolio.Api.Filters;
using Portfolio.Application.Users;
using Portfolio.Application.Users.Requests;
using Portfolio.Application.Validations;
using Portfolio.Domain.Shared;

namespace Portfolio.Api.Endpoints;

public static class UsersEndpoints
{
	public static void MapUsersEndpoints(this IEndpointRouteBuilder app, CancellationToken cancellationToken = default)
	{
		var userGroup = app.MapGroup("/users").WithOpenApi();

		userGroup.RequireAuthorization(Permissions.UserView);

		userGroup.MapPost("/", async (CreateUserRequest req, IUserService service) =>
		{
			return (await service.CreateAsync(req))
				.ToHttpResult(user => Results.Created($"/users/{user.Id}", user));
		}).AddEndpointFilter<ValidationFilter<CreateUserRequest>>();

		userGroup.MapGet("/{id:guid}", async (Guid id, IUserService service) =>
		{
			return (await service.GetByIdAsync(id))
				.ToHttpResult(user => Results.Ok(user), StatusCodes.Status404NotFound);
		});

		userGroup.MapGet("/", async (IUserService service) =>
		{
			return (await service.GetAllAsync())
					.ToHttpResult(users => Results.Ok(users), failureStatusCode: StatusCodes.Status204NoContent);
		});

		userGroup.MapDelete("/{id:guid}", async (Guid id, IUserService service) =>
		{
			return (await service.DeleteAsync(id))
				.ToHttpResult(_ => Results.Ok(), StatusCodes.Status404NotFound);
		});

		userGroup.MapGet("/with-roles", async (IUserRoleService roleService) =>
		{
			return (await roleService.GetAllUsersWithRolesAsync())
				.ToHttpResult(users => Results.Ok(users), failureStatusCode: StatusCodes.Status204NoContent);
		}).RequireAuthorization(Permissions.RolesView);

		userGroup.MapPut("/{id:guid}/roles", async (Guid id, SetUserRolesRequest req, IUserRoleService roleService) =>
		{
			return (await roleService.SetUserRolesAsync(id, req.RoleIds, cancellationToken))
				.ToHttpResult(roles => Results.Ok(roles), StatusCodes.Status404NotFound);
		}).RequireAuthorization(Permissions.RolesUpdate)
		.RequireAuthorization(Permissions.UserUpdate);

	}
}