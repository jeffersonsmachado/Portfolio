namespace Portfolio.Application.Profiles;

/// <summary>
/// Data Transfer Object for Profile entity
/// </summary>
public class ProfileDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
	public string Bio { get; set; } = string.Empty;
    public string BioTitle { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public List<SkillDto> Skills { get; set; } = [];
    public List<ExperienceDto> Experiences { get; set; } = [];
    public List<EducationDto> Educations { get; set; } = [];
	
}
