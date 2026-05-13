using Portfolio.Infrastructure;
using Scalar.AspNetCore;
using FluentValidation;
using Portfolio.Application.Validations;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Data;
using Portfolio.Api.Endpoints;
using Portfolio.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

#region Builder Setup

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<Portfolio.Api.Middlewares.GlobalExceptionHandler>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
	options.AddPolicy("FrontendDev", policy =>
	{
		policy
			.WithOrigins("http://localhost:5173", "http://localhost:5174")
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.MapInboundClaims = false;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
		};

		options.Events = new JwtBearerEvents
		{
			OnTokenValidated = context =>
			{
				var revocationService = context.HttpContext.RequestServices
					.GetRequiredService<ITokenRevocationService>();

				var sub = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

				if (sub != null && Guid.TryParse(sub, out var userId) && revocationService.IsRevoked(userId))
				{
					context.Fail("Token revoked");
				}

				return Task.CompletedTask;
			}
		};
	});

builder.Services.AddPermissionAuthorization();


#endregion

#region App Setup

var app = builder.Build();


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<PortfolioDbContext>();

if (app.Environment.IsEnvironment("Testing"))
{
	await context.Database.EnsureCreatedAsync();
}
else
{
	await context.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
	await app.SeedDatabaseAsync();
	app.MapOpenApi();
	app.MapScalarApiReference();
}


app.UseExceptionHandler();

app.UseCors("FrontendDev");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

#endregion

app.MapProfilesEndpoints();
app.MapUsersEndpoints();
app.MapRolesEndpoints();
app.MapAuthEndpoints();

app.Run();

public partial class Program { }