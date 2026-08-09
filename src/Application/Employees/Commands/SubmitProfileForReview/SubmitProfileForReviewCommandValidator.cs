namespace NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;

public class SubmitProfileForReviewCommandValidator : AbstractValidator<SubmitProfileForReviewCommand>
{
    public SubmitProfileForReviewCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.IqamaNumber).NotEmpty().MaximumLength(30);
    }
}
