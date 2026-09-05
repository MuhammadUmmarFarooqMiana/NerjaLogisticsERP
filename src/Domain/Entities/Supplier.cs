namespace NerjaLogisticsERP.Domain.Entities;

public class Supplier : BaseAuditableEntity
{
    private Supplier() { }
    private Supplier(string name, string? email, string? phone, string? address)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
    }

    public string Name { get; private set; } = default!;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }

    public static Supplier Create(string name, string? email, string? phone, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required.", nameof(name));

        return new Supplier(name, email, phone, address);
    }

    public void Update(string name, string? email, string? phone, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required.", nameof(name));

        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
    }
}
