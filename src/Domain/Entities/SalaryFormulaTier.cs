namespace NerjaLogisticsERP.Domain.Entities;

public class SalaryFormulaTier : BaseEntity
{
    private SalaryFormulaTier() { }
    internal SalaryFormulaTier(Guid salaryFormulaId, int minOrders, int? maxOrders, SalaryTierRateType rateType, decimal rate)
    {
        SalaryFormulaId = salaryFormulaId;
        MinOrders = minOrders;
        MaxOrders = maxOrders;
        RateType = rateType;
        Rate = rate;
    }

    public Guid SalaryFormulaId { get; private set; }
    public int MinOrders { get; private set; }
    public int? MaxOrders { get; private set; }
    public SalaryTierRateType RateType { get; private set; }
    public decimal Rate { get; private set; }
}
