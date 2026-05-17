namespace Portfolio.Application.Profiles;

/// <summary>
/// Data Transfer Object for Experience entity
/// </summary>
public class ExperienceDto
{
    public Guid Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int StartMonth { get; set; }
    public int StartYear { get; set; }
    public int? EndMonth { get; set; }
    public int? EndYear { get; set; }
    public bool Current { get; set; }
    public string Description { get; set; } = string.Empty;
	
}
