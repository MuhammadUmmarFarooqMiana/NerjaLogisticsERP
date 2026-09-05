namespace NerjaLogisticsERP.Application.Platforms.Commands.UpdatePlatform;

public class UpdatePlatformCommandValidator : AbstractValidator<UpdatePlatformCommand>
{
    public UpdatePlatformCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
