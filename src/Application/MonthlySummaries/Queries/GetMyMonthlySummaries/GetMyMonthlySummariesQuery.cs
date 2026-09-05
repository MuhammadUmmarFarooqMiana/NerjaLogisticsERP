using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMyMonthlySummaries;

// No EmployeeId param — always the caller's own monthly summaries.
[Authorize]
public record GetMyMonthlySummariesQuery : IRequest<PaginatedList<MonthlySummaryDto>>
{
    public int? Year { get; init; }
    public int? Month { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
