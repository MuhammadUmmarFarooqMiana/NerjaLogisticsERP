using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetSalaryFormulasQuery : IRequest<PaginatedList<SalaryFormulaDto>>
{
    public Guid? PlatformId { get; init; }
    public bool ActiveOnly { get; init; } = false;
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
