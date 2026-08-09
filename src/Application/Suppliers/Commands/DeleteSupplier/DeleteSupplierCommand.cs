namespace NerjaLogisticsERP.Application.Suppliers.Commands.DeleteSupplier;

public record DeleteSupplierCommand : IRequest
{
    public Guid Id { get; init; }
}
