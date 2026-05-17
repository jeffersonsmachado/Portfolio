namespace Portfolio.Application.Profiles;
/// <summary>
/// Request model for adding a new education to a profile
/// </summary>

public record AddEducationRequest(
    string Institution,
    string Degree,
    int StartMonth,
    int StartYear,
    int? EndMonth,
    int? EndYear
);