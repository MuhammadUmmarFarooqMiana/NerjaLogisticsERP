namespace NerjaLogisticsERP.Application.Employees.Commands.ApproveEmployee;

public record ApproveEmployeeCommand : IRequest
{
    public Guid EmployeeId { get; init; }
    public DateOnly? JoiningDate { get; init; }
}
