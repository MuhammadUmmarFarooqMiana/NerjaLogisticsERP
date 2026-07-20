namespace NerjaLogisticsERP.Domain.Entities;

public class Platform : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty; // Hunger, Jahez, Keeta

    private Platform() { } // EF Core
    private Platform(string name)
    {
        Name = name;
    }
    public static Platform Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Platform name is required.",
                nameof(name));

        return new Platform(name);
    }


    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Platform name is required.",
                nameof(name));

        Name = name;
    }
}
