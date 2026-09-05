using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Fines.Queries.GetFinesReport;

namespace NerjaLogisticsERP.Application.Reports.Fines.Queries.ExportFinesReport;

public class ExportFinesReportQueryHandler : IRequestHandler<ExportFinesReportQuery, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<FinesReportDto> _exporter;

    public ExportFinesReportQueryHandler(ISender sender, IReportExporter<FinesReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportFinesReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetFinesReportQuery
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
        var fileName = $"fines-report_{report.PeriodStart:yyyyMMdd}-{report.PeriodEnd:yyyyMMdd}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
