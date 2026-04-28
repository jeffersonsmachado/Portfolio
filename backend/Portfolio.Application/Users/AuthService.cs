using System.Security.Cryptography;
using Portfolio.Application.Services;
using Portfolio.Application.Users.Dtos;
using Portfolio.Application.Users.Requests;
using Portfolio.Application.Validations;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Services;
using Portfolio.Domain.Shared;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Application.Users;

/// <summary>
/// Service implementation for Authentication-related operations
/// Handles user authentication, including login and email verification.
/// </summary>
/// <param name="userRepository">The user repository for accessing user data.</param>
/// <param name="emailService">The email service for sending verification emails.</param>
/// <param name="jwtProvider">The JWT provider for generating authentication tokens.</param>
/// <param name="passwordHasher">The password hasher for verifying user passwords.</param>
public class AuthService(IUserRepository userRepository, IEmailService emailService, IJwtProvider jwtProvider, IPasswordHasher passwordHasher) : IAuthService
{
	private readonly IUserRepository _userRepository = userRepository;
	private readonly IEmailService _emailService = emailService;
	private readonly IJwtProvider _jwtProvider = jwtProvider;
	private readonly IPasswordHasher _passwordHasher = passwordHasher;

	// Verifies the provided email and token for email verification.
	public async Task<Result<UserDto>> VerifyToken(VerifyTokenRequest request, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
		if (user == null)
		{
			return Result<UserDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
		}

		if (!user.VerifyToken(request.Token))
		{
			return Result<UserDto>.Failure(new Error("INVALID_TOKEN", "Invalid verification token."));
		}

		await _userRepository.UpdateAsync(user, cancellationToken);

		return Result<UserDto>.Success(new UserDto
		{
			Id = user.Id,
			Username = user.Name.Value,
			Email = user.Email.Value
		});
	}

	// Handles user login by verifying email and password, and returns a JWT token if successful.
	public async Task<Result<LoginDto>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
		if (user == null)
		{
			return Result<LoginDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
		}

		if (!user.IsEmailVerified)
		{
			return Result<LoginDto>.Failure(new Error("EMAIL_NOT_VERIFIED", "Email address has not been verified."));
		}

		if (!user.VerifyPassword(request.Password, _passwordHasher))
		{
			return Result<LoginDto>.Failure(new Error("INVALID_CREDENTIALS", "Invalid email or password."));
		}

		var token = _jwtProvider.Generate(user);

		var capabilities = user.Roles
			.SelectMany(r => r.Permissions)
			.Select(p => p.Name)
			.Distinct()
			.Select(p => p.Split(':'))
			.Where(parts => parts.Length == 2)
			.GroupBy(parts => parts[0], parts => parts[1])
			.ToDictionary(
				g => g.Key,
				g => new ResourceCapabilities
				{
					CanCreate = g.Contains("create"),
					CanRead = g.Contains("view"),
					CanUpdate = g.Contains("update"),
					CanDelete = g.Contains("delete")
				}
			);

		return Result<LoginDto>.Success(new LoginDto
		{
			Token = token,
			Capabilities = capabilities
		});
	}

	// Resends a new verification token to the user's email if the email is not yet verified.
	public async Task<Result<string>> ResendVerificationTokenAsync(ResendVerificationTokenRequest request, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
		if (user == null)
		{
			return Result<string>.Failure(new Error("USER_NOT_FOUND", "User not found."));
		}

		if (user.IsEmailVerified)
		{
			return Result<string>.Failure(new Error("EMAIL_ALREADY_VERIFIED", "Email address is already verified."));
		}

		string newToken = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
		user.UpdateVerificationToken(newToken);
		await _userRepository.UpdateAsync(user, cancellationToken);

		string subject = "Your new Portfolio verification token";

		string body = $@"
			<h1>Portfolio Verification Token</h1>
			<p>Hi {user.Name.Value},</p>
			<p>You requested a new verification token. Please use the following token to verify your email address:</p>
			<br/><h2>{newToken}</h2><br/>
			<p>This token will expire in 24 hours.</p>
			<br/>
			<p>Best regards,<br/>The Portfolio Team</p>
		";

		await _emailService.SendEmailAsync(user.Email.Value, subject, body, cancellationToken);

		return Result<string>.Success("Verification token resent successfully.");
	}

	public async Task<Result<UserDto>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
	{
		// Check if username already exists
		var existingUser = await _userRepository.GetByNameAsync(request.Username, cancellationToken);
		if (existingUser != null)
		{
			return Result<UserDto>.Failure(new Error("USERNAME_ALREADY_EXISTS", "Username already exists."));
		}

		var existingEmail = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
		if (existingEmail != null)
		{
			return Result<UserDto>.Failure(new Error("EMAIL_ALREADY_EXISTS", "Email already exists."));
		}

		// Generate 6 digit verification token
		string verificationToken = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

		// Create new User entity
		var passwordHash = PasswordHash.CreateFromRaw(request.Password, _passwordHasher);
		if (passwordHash.IsFailure)
			return Result<UserDto>.Failure(passwordHash.Error!);

		User user = User.Create(
			UserName.Parse(request.Username),
			Email.Parse(request.Email),
			passwordHash.Value!,
			verificationToken
		).Value!;

		// Save to repository
		await _userRepository.AddAsync(user, cancellationToken);


		string subject = "Welcome to Portfolio!";

		string body = $@"
			<h1>Welcome to Portfolio!</h1>
			<p>Hi {user.Name.Value},</p>
			<p>Thank you for registering an account with us.</p>
			<p>Please use the following verification token to verify your email address:</p>
			<br/><h2>{verificationToken}</h2><br/>
			<p>This token will expire in 24 hours.</p>
			<br/>
			<p>Best regards,<br/>The Portfolio Team</p>
		";

		await _emailService.SendEmailAsync(user.Email.Value, subject, body, cancellationToken);

		return Result<UserDto>.Success(new UserDto
		{
			Id = user.Id,
			Username = user.Name.Value,
			Email = user.Email.Value
		});
	}
}