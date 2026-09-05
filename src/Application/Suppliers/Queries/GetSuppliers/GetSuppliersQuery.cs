using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Suppliers.Queries.GetSuppliers;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetSuppliersQuery : IRequest<PaginatedList<SupplierDto>>
{
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
