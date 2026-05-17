namespace Portfolio.Application.Profiles;
/// <summary>
/// Request model for updating an existing education in a profile
/// </summary>

public record UpdateEducationRequest(
    string Institution,
    string Degree,
    int StartMonth,
    int StartYear,
    int? EndMonth,
    int? EndYear
);