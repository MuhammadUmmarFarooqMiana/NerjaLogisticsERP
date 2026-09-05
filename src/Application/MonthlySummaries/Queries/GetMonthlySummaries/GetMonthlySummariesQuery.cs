using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaries;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetMonthlySummariesQuery : IRequest<PaginatedList<MonthlySummaryDto>>
{
    public Guid? EmployeeId { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public MonthlySummaryStatus? Status { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
