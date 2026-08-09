using NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;

namespace NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulaById;


public record GetSalaryFormulaByIdQuery : IRequest<SalaryFormulaDto>
{
    public Guid Id { get; init; }
}
