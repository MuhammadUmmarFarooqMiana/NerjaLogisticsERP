using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Suppliers.Queries.GetSuppliersReport;

namespace NerjaLogisticsERP.Application.Reports.Suppliers.Queries.ExportSuppliersReport;

public class ExportSuppliersReportQueryHandler : IRequestHandler<ExportSuppliersReportQuery, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<SuppliersReportDto> _exporter;

    public ExportSuppliersReportQueryHandler(ISender sender, IReportExporter<SuppliersReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportSuppliersReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetSuppliersReportQuery
        {
            PeriodType = request.PeriodType,
            Date = request.Date,
            Year = request.Year,
            Month = request.Month,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            SupplierId = request.SupplierId
        }, cancellationToken);

        var isPdf = request.Format == ReportFormat.Pdf;
        var content = isPdf ? _exporter.GeneratePdf(report) : _exporter.GenerateExcel(report);
        var contentType = isPdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = isPdf ? "pdf" : "xlsx";
        var fileName = $"suppliers-report_{report.PeriodStart:yyyyMMdd}-{report.PeriodEnd:yyyyMMdd}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
