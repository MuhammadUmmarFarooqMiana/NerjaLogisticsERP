using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Orders.Queries.GetOrdersReport;

public class GetOrdersReportQueryValidator : AbstractValidator<GetOrdersReportQuery>
{
    public GetOrdersReportQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .NotNull()
            .When(x => x.PeriodType == ReportPeriodType.Custom)
            .WithMessage("Custom reports require a start date.");

        RuleFor(x => x.EndDate)
            .NotNull()
            .When(x => x.PeriodType == ReportPeriodType.Custom)
            .WithMessage("Custom reports require an end date.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .When(x => x.Month.HasValue)
            .WithMessage("Month must be between 1 and 12.");
    }
}
