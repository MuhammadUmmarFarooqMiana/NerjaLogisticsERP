using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Fines.Queries.GetFines;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetFinesQuery : IRequest<PaginatedList<FineDto>>
{
    public Guid? EmployeeId { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
