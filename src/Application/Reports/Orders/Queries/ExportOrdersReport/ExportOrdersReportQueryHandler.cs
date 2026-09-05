using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Orders.Queries.GetOrdersReport;

namespace NerjaLogisticsERP.Application.Reports.Orders.Queries.ExportOrdersReport;

public class ExportOrdersReportQueryHandler : IRequestHandler<ExportOrdersReportQuery, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<OrdersReportDto> _exporter;

    public ExportOrdersReportQueryHandler(ISender sender, IReportExporter<OrdersReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportOrdersReportQuery request, CancellationToken cancellationToken)
    {
        // Reuses the same query (and its own authorization/validation/self-scoping)
        // that powers the on-screen preview, so the exported file can never diverge
        // from what the user reviewed before exporting it.
        var report = await _sender.Send(new GetOrdersReportQuery
        {
            PeriodType = request.PeriodType,
            Date = request.Date,
            Year = request.Year,
            Month = request.Month,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            PlatformId = request.PlatformId,
            EmployeeId = request.EmployeeId
        }, cancellationToken);

        var isPdf = request.Format == ReportFormat.Pdf;
        var content = isPdf ? _exporter.GeneratePdf(report) : _exporter.GenerateExcel(report);
        var contentType = isPdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = isPdf ? "pdf" : "xlsx";
        var fileName = $"orders-report_{report.PeriodStart:yyyyMMdd}-{report.PeriodEnd:yyyyMMdd}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
