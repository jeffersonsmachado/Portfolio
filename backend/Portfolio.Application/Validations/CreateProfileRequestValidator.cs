using FluentValidation;
using Portfolio.Application.Profiles;
using Portfolio.Domain.Aggregates.Profiles;

namespace Portfolio.Application.Validations;

public class CreateProfileRequestValidator : AbstractValidator<CreateProfileRequest>
{
	public CreateProfileRequestValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
				.WithMessage("Name is required.")
			.Length(ProfileConstants.MinNameLength, ProfileConstants.MaxNameLength)
				.WithMessage($"Name must be between {ProfileConstants.MinNameLength} and {ProfileConstants.MaxNameLength} characters long.")
			.Matches(ProfileConstants.NamePattern)
				.WithMessage("Name can only contain alphanumeric characters, underscores, and hyphens.");

		RuleFor(x => x.UserId)
			.NotEmpty()
				.WithMessage("UserId is required.")
			.Must(userId => userId != Guid.Empty)
				.WithMessage("UserId cannot be an empty GUID.");
	}
}