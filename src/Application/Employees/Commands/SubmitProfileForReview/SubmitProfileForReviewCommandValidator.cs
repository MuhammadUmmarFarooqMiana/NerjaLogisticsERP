namespace NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;

public class SubmitProfileForReviewCommandValidator : AbstractValidator<SubmitProfileForReviewCommand>
{
    public SubmitProfileForReviewCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.IqamaNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PlatformIdNumber).NotEmpty().MaximumLength(50)
            .WithMessage("Platform ID number is required.");
    }
}
