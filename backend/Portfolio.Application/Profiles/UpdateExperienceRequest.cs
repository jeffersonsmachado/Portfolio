namespace Portfolio.Application.Profiles;
/// <summary>
/// Request model for updating an existing experience in a profile
/// </summary>

public record UpdateExperienceRequest(
    string Company,
    string Role,
    int StartMonth,
    int StartYear,
    int? EndMonth,
    int? EndYear,
    bool Current,
    string Description
);