namespace NerjaLogisticsERP.Application.Advances.Commands.CreateAdvance;

public class CreateAdvanceCommandValidator : AbstractValidator<CreateAdvanceCommand>
{
    public CreateAdvanceCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

