using NerjaLogisticsERP.Application.Suppliers.Queries.GetSuppliers;

namespace NerjaLogisticsERP.Application.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery : IRequest<SupplierDto>
{
    public Guid Id { get; init; }
}
