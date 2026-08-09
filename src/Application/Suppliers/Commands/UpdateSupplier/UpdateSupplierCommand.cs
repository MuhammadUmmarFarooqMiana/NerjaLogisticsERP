namespace NerjaLogisticsERP.Application.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
}
