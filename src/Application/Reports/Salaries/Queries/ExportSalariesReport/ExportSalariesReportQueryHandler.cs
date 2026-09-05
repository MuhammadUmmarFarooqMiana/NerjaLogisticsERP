using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Salaries.Queries.GetSalariesReport;

namespace NerjaLogisticsERP.Application.Reports.Salaries.Queries.ExportSalariesReport;

public class ExportSalariesReportQueryHandler : IRequestHandler<ExportSalariesReportQuery, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<SalariesReportDto> _exporter;

    public ExportSalariesReportQueryHandler(ISender sender, IReportExporter<SalariesReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportSalariesReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetSalariesReportQuery
        {
            PeriodType = request.PeriodType,
            Date = request.Date,
            Year = request.Year,
            Month = request.Month,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            EmployeeId = request.EmployeeId
        }, cancellationToken);

        var isPdf = request.Format == ReportFormat.Pdf;
        var content = isPdf ? _exporter.GeneratePdf(report) : _exporter.GenerateExcel(report);
        var contentType = isPdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = isPdf ? "pdf" : "xlsx";
        var fileName = $"salaries-report_{report.PeriodStart:yyyyMMdd}-{report.PeriodEnd:yyyyMMdd}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
