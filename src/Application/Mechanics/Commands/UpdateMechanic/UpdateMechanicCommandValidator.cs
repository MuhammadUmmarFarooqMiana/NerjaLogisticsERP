namespace NerjaLogisticsERP.Application.Mechanics.Commands.UpdateMechanic;

public class UpdateMechanicCommandValidator : AbstractValidator<UpdateMechanicCommand>
{
    public UpdateMechanicCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Specialty).MaximumLength(200);
        RuleFor(x => x.Address).MaximumLength(500);
    }
}
