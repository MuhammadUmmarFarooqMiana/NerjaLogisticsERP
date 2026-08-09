namespace NerjaLogisticsERP.Application.Salaries.Commands.DeactivateSalaryFormula;

public record DeactivateSalaryFormulaCommand : IRequest
{
    public Guid Id { get; init; }
}
