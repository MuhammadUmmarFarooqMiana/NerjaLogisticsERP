using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Employees.Queries;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetEmployees;

// Administrator/Supervisor use this for the Employees list itself; Accountant
// doesn't get that page, but needs the same data to populate employee pickers
// on Fines/Advances/Monthly Summaries/Leave Requests (all Accountant-facing).
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor},{Roles.Accountant}")]
public record GetEmployeesQuery : IRequest<PaginatedList<EmployeeListItemDto>>
{
    public AccountStatus? Status { get; init; }
    public Guid? PlatformId { get; init; }
    public string? SearchTerm { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
