using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanics;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetMechanicsQuery : IRequest<PaginatedList<MechanicDto>>
{
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
