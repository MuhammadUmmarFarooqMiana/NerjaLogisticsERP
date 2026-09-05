using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.ExportReconciliationReport;

public class ExportReconciliationReportCommandValidator : AbstractValidator<ExportReconciliationReportCommand>
{
    public ExportReconciliationReportCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x).Must(x => AllowedFileTypes.IsAllowed(x.ContentType, x.FileName, x.Content.Length))
            .WithMessage("File must be an Excel workbook (.xlsx) no larger than 10 MB.");

        RuleFor(x => x.PlatformId).NotEmpty();
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
    }
}
