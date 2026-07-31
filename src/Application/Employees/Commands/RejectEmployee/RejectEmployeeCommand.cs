namespace NerjaLogisticsERP.Application.Employees.Commands.RejectEmployee;

public record RejectEmployeeCommand : IRequest
{
    public Guid EmployeeId { get; init; }
    public string Reason { get; init; } = default!;
}
