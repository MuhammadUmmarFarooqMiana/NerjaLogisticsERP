using FluentValidation;

namespace NerjaLogisticsERP.Application.Employees.Commands.RejectEmployee;

public class RejectEmployeeCommandValidator : AbstractValidator<RejectEmployeeCommand>
{
    public RejectEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(5000);
    }
}
