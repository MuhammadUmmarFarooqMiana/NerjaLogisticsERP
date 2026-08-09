namespace NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;

public record GetSalaryFormulasQuery : IRequest<List<SalaryFormulaDto>>
{
    public Guid? PlatformId { get; init; }
    public bool ActiveOnly { get; init; } = false;
}
