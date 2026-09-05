using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Advances.Queries.GetAdvancesReport;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Advances.Queries.ExportAdvancesReport;

public class ExportAdvancesReportQueryHandler : IRequestHandler<ExportAdvancesReportQuery, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<AdvancesReportDto> _exporter;

    public ExportAdvancesReportQueryHandler(ISender sender, IReportExporter<AdvancesReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportAdvancesReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetAdvancesReportQuery
        {
            PeriodType = request.PeriodType,
            Date = request.Date,
            Year = request.Year,
            Month = request.Month,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            EmployeeId = request.EmployeeId
        }, cancellationToken);

        var isPdf = request.Format == ReportFormat.Pdf;
        var content = isPdf ? _exporter.GeneratePdf(report) : _exporter.GenerateExcel(report);
        var contentType = isPdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = isPdf ? "pdf" : "xlsx";
        var fileName = $"advances-report_{report.PeriodStart:yyyyMMdd}-{report.PeriodEnd:yyyyMMdd}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
