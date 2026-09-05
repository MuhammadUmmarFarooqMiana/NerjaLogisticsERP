using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Suppliers.Queries.GetSuppliers;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Suppliers.Queries.GetSupplierById;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetSupplierByIdQuery : IRequest<SupplierDto>
{
    public Guid Id { get; init; }
}
