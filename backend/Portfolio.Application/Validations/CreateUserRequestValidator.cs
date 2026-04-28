using FluentValidation;
using Portfolio.Application.Users;
using Portfolio.Application.Users.Requests;
using Portfolio.Domain.Aggregates.Users;

namespace Portfolio.Application.Validations;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
	public CreateUserRequestValidator()
	{
		RuleFor(x => x.Username)
			.NotEmpty().WithMessage("Username is required.")
			.Length(UserConstants.MinNameLength, UserConstants.MaxNameLength)
				.WithMessage($"Username must be between {UserConstants.MinNameLength} and {UserConstants.MaxNameLength} characters long.")
			.Matches(UserConstants.NamePattern)
				.WithMessage("Username can only contain alphanumeric characters, underscores, and hyphens.");

		RuleFor(x => x.Email)
			.NotEmpty()
				.WithMessage("Email is required.")
			.EmailAddress()
				.WithMessage("Invalid email format.");

		RuleFor(x => x.Password)
			.NotEmpty()
				.WithMessage("Password is required.")
			.MinimumLength(UserConstants.MinPasswordLength)
				.WithMessage($"Password must be at least {UserConstants.MinPasswordLength} characters long.")
			.MaximumLength(UserConstants.MaxPasswordLength)
				.WithMessage($"Password cannot exceed {UserConstants.MaxPasswordLength} characters.");

		RuleFor(x => x.ConfirmPassword)
			.NotEmpty()
				.WithMessage("Confirm Password is required.")
			.Equal(x => x.Password)
				.WithMessage("Passwords do not match.");
	}
}