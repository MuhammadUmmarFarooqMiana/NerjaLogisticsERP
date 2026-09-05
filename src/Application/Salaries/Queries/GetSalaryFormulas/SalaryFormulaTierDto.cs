namespace NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;

public record SalaryFormulaTierDto
{
    public int MinOrders { get; init; }
    public int? MaxOrders { get; init; }
    public string RateType { get; init; } = default!;
    public decimal Rate { get; init; }
}
