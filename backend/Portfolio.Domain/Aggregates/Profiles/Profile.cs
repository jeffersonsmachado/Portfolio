using Portfolio.Domain.Aggregates.Users;

namespace Portfolio.Domain.Aggregates.Profiles;

public partial class Profile
{
	#region Properties

	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public string Bio { get; private set; } = string.Empty;
	public string BioTitle { get; private set; } = string.Empty;
	public string AvatarUrl { get; private set; } = string.Empty;

	#endregion

	#region Navigation Properties

	public virtual User User { get; set; } = null!;
	public virtual ICollection<Skill> Skills { get; set; } = [];
	public virtual ICollection<Experience> Experiences { get; set; } = [];
	public virtual ICollection<Education> Educations { get; set; } = [];

	#endregion

	#region EF Core Constructor
	private Profile()
	{
		Name = string.Empty;
	}

	private Profile(User user)
	{
		Name = string.Empty;
		User = user;
	}

	private Profile(string name, User user)
	{
		Id = Guid.NewGuid();
		Name = name;
		User = user;
	}

	public static Profile Create(string name, User user)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Profile name cannot be empty", nameof(name));
		}
		return new Profile(name, user);
	}

	public void UpdateProfile(string name, string? bio, string? bioTitle, string? avatarUrl)
	{
		Name = name;
		Bio = bio ?? string.Empty;
		BioTitle = bioTitle ?? string.Empty;
		AvatarUrl = avatarUrl ?? string.Empty;
	}

	#endregion

	#region Factory Methods

	// Factory method for recreating from storage (or mock data)
	public static Profile Recreate(Guid id, string name, User user, string bio = "", string bioTitle = "", string avatarUrl = "")
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException("Id cannot be empty", nameof(id));
		}

		return new Profile(name, user)
		{
			Id = id,
			Bio = bio,
			BioTitle = bioTitle,
			AvatarUrl = avatarUrl
		};
	}

	#endregion

	#region Domain Methods

	public Skill? AddSkill(string name)
	{
		if (string.IsNullOrWhiteSpace(name) || Skills.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
		{
			return null;
		}
		Skill skill = Skill.Create(name, Id);
		Skills.Add(skill);
		return skill;
	}

	public void RemoveSkill(Guid skillId)
	{
		var skill = Skills.FirstOrDefault(s => s.Id == skillId);
		if (skill != null)
		{
			Skills.Remove(skill);
		}
	}

	public Experience? AddExperience(string company, string role, int startMonth, int startYear, int? endMonth, int? endYear, bool current, string description)
	{
		if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(role))
		{
			return null;
		}
		Experience experience = Experience.Create(company, role, startMonth, startYear, endMonth, endYear, current, description, Id);
		Experiences.Add(experience);
		return experience;
	}

	public Experience? UpdateExperience(Guid experienceId, string company, string role, int startMonth, int startYear, int? endMonth, int? endYear, bool current, string description)
	{
		var experience = Experiences.FirstOrDefault(e => e.Id == experienceId);
		if (experience != null)
		{
			experience.Update(company, role, startMonth, startYear, endMonth, endYear, current, description);
			return experience;
		}
		return null;
	}

	public void RemoveExperience(Guid experienceId)
	{
		var experience = Experiences.FirstOrDefault(e => e.Id == experienceId);
		if (experience != null)
		{
			Experiences.Remove(experience);
		}
	}

	public Education? AddEducation(string institution, string degree, int startMonth, int startYear, int? endMonth, int? endYear)
	{
		if (string.IsNullOrWhiteSpace(institution) || string.IsNullOrWhiteSpace(degree))
		{
			return null;
		}
		Education education = Education.Create(institution, degree, startMonth, startYear, endMonth, endYear, Id);
		Educations.Add(education);
		return education;
	}

	public Education? UpdateEducation(Guid educationId, string institution, string degree, int startMonth, int startYear, int? endMonth, int? endYear)
	{
		var education = Educations.FirstOrDefault(e => e.Id == educationId);
		if (education != null)
		{
			education.Update(institution, degree, startMonth, startYear, endMonth, endYear);
			return education;
		}
		return null;
	}

	public void RemoveEducation(Guid educationId)
	{
		var education = Educations.FirstOrDefault(e => e.Id == educationId);
		if (education != null)
		{
			Educations.Remove(education);
		}
	}

	#endregion
}
