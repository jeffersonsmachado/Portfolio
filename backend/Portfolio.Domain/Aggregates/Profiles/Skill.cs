namespace Portfolio.Domain.Aggregates.Profiles;

public class Skill
{
    #region Properties

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid ProfileId { get; private set; }

    #endregion

    #region Factory Methods

    private Skill() { }

    public static Skill Create(string name, Guid profileId)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            ProfileId = profileId
        };
    }

    public void Update(string name)
    {
        Name = name;
    }

    #endregion
}