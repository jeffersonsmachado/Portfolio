using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Portfolio.Application.Services;
using Portfolio.Domain.Aggregates.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Portfolio.Infrastructure.Services;

// <summary>
// Implementation of IJwtProvider for JWT token generation
// </summary>
//
// <param name="configuration">Application configuration for accessing JWT settings</param>
// <returns>Generated JWT token as a string</returns>
public class JwtProvider(IConfiguration configuration) : IJwtProvider
{
	public string Generate(User user)
	{
		var secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret key is not configured.");
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		// Claims added to the token
		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

			new(ClaimTypes.NameIdentifier, user.Id.ToString()),

			new(JwtRegisteredClaimNames.Email, user.Email.Value),

			new(JwtRegisteredClaimNames.Name, user.Name.Value),

			new("portfolio_username", user.Name.Value)
		};

		var permissions = user.Roles
			.SelectMany(r => r.Permissions)
			.Select(p => p.Name)
			.Distinct();

		foreach (var permission in permissions)
		{
			claims.Add(new Claim("permission", permission));
		}

		var token = new JwtSecurityToken(
			issuer: configuration["Jwt:Issuer"],
			audience: configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddHours(1),
			signingCredentials: credentials
		);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}