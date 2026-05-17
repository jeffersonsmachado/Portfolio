using System.Security.Claims;
using Portfolio.Api.Extensions;
using Portfolio.Api.Filters;
using Portfolio.Application.Users;
using Portfolio.Application.Users.Requests;
using Portfolio.Application.Validations;

namespace Portfolio.Api.Endpoints;

public static class AuthEndpoints
{
	public static void MapAuthEndpoints(this IEndpointRouteBuilder app, CancellationToken cancellationToken = default)
	{
		var authGroup = app.MapGroup("/auth").WithOpenApi();

		authGroup.MapPost("/register", async (RegisterRequest req, IAuthService authService) =>
		{
			var result = await authService.RegisterAsync(req, cancellationToken);
			return result.ToHttpResult(user => Results.Created($"/users/{user.Id}", user), StatusCodes.Status400BadRequest);
		}).AddEndpointFilter<ValidationFilter<RegisterRequest>>();

		authGroup.MapPost("/login", async (LoginRequest req, IAuthService authService) =>
		{
			var result = await authService.LoginAsync(req, cancellationToken);
			return result.ToHttpResult(loginDto => Results.Ok(loginDto), StatusCodes.Status401Unauthorized);
		}).AddEndpointFilter<ValidationFilter<LoginRequest>>();

		authGroup.MapPost("/verify", async (VerifyTokenRequest req, IAuthService authService) =>
		{
			return (await authService.VerifyToken(req, cancellationToken))
				.ToHttpResult(user => Results.Ok(user), StatusCodes.Status400BadRequest);
		}).AddEndpointFilter<ValidationFilter<VerifyTokenRequest>>();

		authGroup.MapPost("/resend-verification", async (ResendVerificationTokenRequest req, IAuthService authService) =>
		{
			var result = await authService.ResendVerificationTokenAsync(req, cancellationToken);
			return result.ToHttpResult(_ => Results.Ok(), StatusCodes.Status400BadRequest);
		}).AddEndpointFilter<ValidationFilter<ResendVerificationTokenRequest>>();

		authGroup.MapGet("/me", async (ClaimsPrincipal user, IAuthService authService) =>
		{
			var claim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier ||
				c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

			if (!Guid.TryParse(claim?.Value, out var userId))
			{
				return Results.Unauthorized();
			}

			return (await authService.GetCapabilitiesAsync(userId, cancellationToken))
				.ToHttpResult(capabilities => Results.Ok(capabilities), StatusCodes.Status401Unauthorized);
		}).RequireAuthorization();
	}
}