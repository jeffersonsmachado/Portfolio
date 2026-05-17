namespace Portfolio.Application.Profiles;
/// <summary>
/// Request model for adding a new experience to a profile
/// </summary>

public record AddExperienceRequest(
    string Company,
    string Role,
    int StartMonth,
    int StartYear,
    int? EndMonth,
    int? EndYear,
    bool Current,
    string Description
);