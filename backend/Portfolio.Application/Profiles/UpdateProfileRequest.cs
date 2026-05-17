namespace Portfolio.Application.Profiles;
/// <summary>
/// Request model for updating an existing profile
/// </summary>

public record UpdateProfileRequest(
    string Name,
    string? Bio,
    string? BioTitle,
    string? AvatarUrl
);