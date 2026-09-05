using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Inventory.Queries.GetInventoryLedgerReport;

namespace NerjaLogisticsERP.Application.Reports.Inventory.Queries.ExportInventoryLedgerReport;

public class ExportInventoryLedgerReportQueryHandler : IRequestHandler<ExportInventoryLedgerReportQuery, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<InventoryLedgerReportDto> _exporter;

    public ExportInventoryLedgerReportQueryHandler(ISender sender, IReportExporter<InventoryLedgerReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportInventoryLedgerReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetInventoryLedgerReportQuery
        {
            PeriodType = request.PeriodType,
            Date = request.Date,
            Year = request.Year,
            Month = request.Month,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ItemId = request.ItemId
        }, cancellationToken);

        var isPdf = request.Format == ReportFormat.Pdf;
        var content = isPdf ? _exporter.GeneratePdf(report) : _exporter.GenerateExcel(report);
        var contentType = isPdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = isPdf ? "pdf" : "xlsx";
        var fileName = $"inventory-ledger-report_{report.PeriodStart:yyyyMMdd}-{report.PeriodEnd:yyyyMMdd}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
