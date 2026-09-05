namespace NerjaLogisticsERP.Application.Employees.Commands.ApproveEmployee;

public class ApproveEmployeeCommandValidator : AbstractValidator<ApproveEmployeeCommand>
{
    public ApproveEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
    }
}
