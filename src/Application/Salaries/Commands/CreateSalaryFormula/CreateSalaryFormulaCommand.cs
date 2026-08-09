using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Salaries.Commands.CreateSalaryFormula;

public record SalaryTierInput(int MinOrders, int? MaxOrders, SalaryTierRateType RateType, decimal Rate);


public record CreateSalaryFormulaCommand : IRequest<Guid>
{
    public Guid? PlatformId { get; init; }
    public SalaryFormulaType FormulaType { get; init; }
    public decimal? FixedMonthlyAmount { get; init; }
    public DateOnly EffectiveFrom { get; init; }
    public List<SalaryTierInput> Tiers { get; init; } = new();
}
