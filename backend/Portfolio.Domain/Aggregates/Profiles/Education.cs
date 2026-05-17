namespace Portfolio.Domain.Aggregates.Profiles;

public class Education
{
    #region Properties
    
    public Guid Id { get; private set; }
    public string Institution { get; private set; } = string.Empty;
    public string Degree { get; private set; } = string.Empty;
    public int StartMonth { get; private set; }
    public int StartYear { get; private set; }
    public int? EndMonth { get; private set; }
    public int? EndYear { get; private set; }
    public Guid ProfileId { get; private set; }
    
    #endregion

    #region Factory Methods

    private Education() { }

    public static Education Create(string institution, string degree, int startMonth, int startYear, int? endMonth, int? endYear, Guid profileId)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Institution = institution,
            Degree = degree,
            StartMonth = startMonth,
            StartYear = startYear,
            EndMonth = endMonth,
            EndYear = endYear,
            ProfileId = profileId
        };
    }

    public void Update(string institution, string degree, int startMonth, int startYear, int? endMonth, int? endYear)
    {
        Institution = institution;
        Degree = degree;
        StartMonth = startMonth;
        StartYear = startYear;
        EndMonth = endMonth;
        EndYear = endYear;
    }

    #endregion
}