namespace Portfolio.Application.Profiles;
/// <summary>
/// Request model for adding a new skill to a profile
/// </summary>

public record AddSkillRequest(
    string Name
);