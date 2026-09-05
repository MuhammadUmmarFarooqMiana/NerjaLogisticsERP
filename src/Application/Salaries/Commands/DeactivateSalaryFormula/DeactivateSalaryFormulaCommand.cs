using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Salaries.Commands.DeactivateSalaryFormula;

[Authorize(Roles = Roles.Administrator)]
public record DeactivateSalaryFormulaCommand : IRequest
{
    public Guid Id { get; init; }
}
