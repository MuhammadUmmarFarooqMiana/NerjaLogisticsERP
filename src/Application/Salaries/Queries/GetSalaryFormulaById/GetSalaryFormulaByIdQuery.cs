using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulaById;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetSalaryFormulaByIdQuery : IRequest<SalaryFormulaDto>
{
    public Guid Id { get; init; }
}
