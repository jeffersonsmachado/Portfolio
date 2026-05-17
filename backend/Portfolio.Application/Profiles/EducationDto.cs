namespace Portfolio.Application.Profiles;

/// <summary>
/// Data Transfer Object for Education entity
/// </summary>
public class EducationDto
{
    public Guid Id { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public int StartMonth { get; set; }
    public int StartYear { get; set; }
    public int? EndMonth { get; set; }
    public int? EndYear { get; set; }
	
}
