namespace NerjaLogisticsERP.Application.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
}
