namespace NerjaLogisticsERP.Domain.Services;

public interface ISalaryCalculator
{
    decimal Calculate(SalaryFormula formula, int completedOrders);
}
