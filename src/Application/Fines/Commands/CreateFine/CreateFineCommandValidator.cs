namespace NerjaLogisticsERP.Application.Fines.Commands.CreateFine;

public class CreateFineCommandValidator : AbstractValidator<CreateFineCommand>
{
    public CreateFineCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(255);
        RuleFor(x => x.FineDate).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
