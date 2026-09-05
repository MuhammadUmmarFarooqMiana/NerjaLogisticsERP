namespace NerjaLogisticsERP.Application.Fines.Queries.GetFines;

public record FineDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = default!;
    public decimal Amount { get; init; }
    public string Reason { get; init; } = default!;
    public DateOnly FineDate { get; init; }
}
