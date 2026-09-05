using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Fines.Queries.GetFines;

namespace NerjaLogisticsERP.Application.Fines.Queries.GetMyFines;

// No EmployeeId param — always the caller's own fines.
[Authorize]
public record GetMyFinesQuery : IRequest<PaginatedList<FineDto>>
{
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
