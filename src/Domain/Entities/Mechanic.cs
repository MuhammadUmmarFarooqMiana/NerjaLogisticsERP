namespace NerjaLogisticsERP.Domain.Entities;

public class Mechanic : BaseAuditableEntity
{
    private Mechanic() { }
    private Mechanic(string name, string? phone, string? email, string? specialty, string? address)
    {
        Name = name;
        Phone = phone;
        Email = email;
        Specialty = specialty;
        Address = address;
    }

    public string Name { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Specialty { get; private set; }
    public string? Address { get; private set; }

    public static Mechanic Create(string name, string? phone, string? email, string? specialty, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Mechanic name is required.", nameof(name));

        return new Mechanic(name, phone, email, specialty, address);
    }

    public void Update(string name, string? phone, string? email, string? specialty, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Mechanic name is required.", nameof(name));

        Name = name;
        Phone = phone;
        Email = email;
        Specialty = specialty;
        Address = address;
    }
}
