namespace Portfolio.Domain.Aggregates.Profiles;

public class Experience
{
    #region Properties

    public Guid Id { get; private set; }
    public string Company { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public int StartMonth { get; private set; }
    public int StartYear { get; private set; }
    public int? EndMonth { get; private set; }
    public int? EndYear { get; private set; }
    public bool Current { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid ProfileId { get; private set; }

    #endregion

    #region Factory Methods

    private Experience() { }

    public static Experience Create(string company, string role, int startMonth, int startYear, int? endMonth, int? endYear, bool current, string description, Guid profileId)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Company = company,
            Role = role,
            StartMonth = startMonth,
            StartYear = startYear,
            EndMonth = endMonth,
            EndYear = endYear,
            Current = current,
            Description = description,
            ProfileId = profileId
        };
    }

    public void Update(string company, string role, int startMonth, int startYear, int? endMonth, int? endYear, bool current, string description)
    {
        Company = company;
        Role = role;
        StartMonth = startMonth;
        StartYear = startYear;
        EndMonth = endMonth;
        EndYear = endYear;
        Current = current;
        Description = description;
    }

    #endregion
}