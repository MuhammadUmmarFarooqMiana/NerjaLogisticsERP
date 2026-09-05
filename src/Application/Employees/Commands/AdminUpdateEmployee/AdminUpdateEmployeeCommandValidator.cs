namespace NerjaLogisticsERP.Application.Employees.Commands.AdminUpdateEmployee;

public class AdminUpdateEmployeeCommandValidator : AbstractValidator<AdminUpdateEmployeeCommand>
{
    public AdminUpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
    }
}
