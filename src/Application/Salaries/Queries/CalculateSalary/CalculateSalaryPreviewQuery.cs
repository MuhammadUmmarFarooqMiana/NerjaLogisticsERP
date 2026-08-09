namespace NerjaLogisticsERP.Application.Salaries.Queries.CalculateSalary;

public record SalaryPreviewDto(Guid EmployeeId, int Year, int Month, int TotalOrders, decimal CalculatedSalary);

public record CalculateSalaryPreviewQuery : IRequest<SalaryPreviewDto>
{
    public Guid EmployeeId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
}
