namespace NerjaLogisticsERP.Application.Advances.Queries.GetAdvances;

public record AdvanceDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = default!;
    public decimal Amount { get; init; }
    public DateOnly AdvanceDate { get; init; }
    public string? Remarks { get; init; }
}
