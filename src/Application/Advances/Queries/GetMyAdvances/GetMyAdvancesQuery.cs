using NerjaLogisticsERP.Application.Advances.Queries.GetAdvances;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Advances.Queries.GetMyAdvances;

// No EmployeeId param — always the caller's own advances.
[Authorize]
public record GetMyAdvancesQuery : IRequest<PaginatedList<AdvanceDto>>
{
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
