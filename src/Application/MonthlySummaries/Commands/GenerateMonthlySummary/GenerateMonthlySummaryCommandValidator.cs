namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;

public class GenerateMonthlySummaryCommandValidator : AbstractValidator<GenerateMonthlySummaryCommand>
{
    public GenerateMonthlySummaryCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).GreaterThan(2020);
    }
}
