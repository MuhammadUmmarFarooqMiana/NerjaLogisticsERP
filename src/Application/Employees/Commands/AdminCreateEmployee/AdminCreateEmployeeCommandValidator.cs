namespace NerjaLogisticsERP.Application.Employees.Commands.AdminCreateEmployee;

public class AdminCreateEmployeeCommandValidator : AbstractValidator<AdminCreateEmployeeCommand>
{
    public AdminCreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Roles).NotEmpty().WithMessage("At least one role is required.");
        RuleFor(x => x.IqamaNumber).NotEmpty();
        RuleFor(x => x.PlatformIdNumber).NotEmpty().When(x => x.PlatformId.HasValue);
    }
}
