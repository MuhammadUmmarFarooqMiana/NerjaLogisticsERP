namespace NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;

public record SalaryFormulaDto
{
    public Guid Id { get; init; }
    public Guid? PlatformId { get; init; }
    public string? PlatformName { get; init; }
    public string FormulaType { get; init; } = default!;
    public decimal? FixedMonthlyAmount { get; init; }
    public DateOnly EffectiveFrom { get; init; }
    public DateOnly? EffectiveTo { get; init; }
    public List<SalaryFormulaTierDto> Tiers { get; init; } = new();
}
