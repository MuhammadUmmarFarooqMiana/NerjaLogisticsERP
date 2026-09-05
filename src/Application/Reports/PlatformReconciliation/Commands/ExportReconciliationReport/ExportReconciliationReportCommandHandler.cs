using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.GenerateReconciliationReport;

namespace NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.ExportReconciliationReport;

public class ExportReconciliationReportCommandHandler : IRequestHandler<ExportReconciliationReportCommand, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<PlatformReconciliationReportDto> _exporter;

    public ExportReconciliationReportCommandHandler(ISender sender, IReportExporter<PlatformReconciliationReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportReconciliationReportCommand request, CancellationToken cancellationToken)
    {
        // Re-runs the same parse+compare the preview endpoint does (the uploaded file is
        // stateless on the server — nothing was persisted from the earlier preview call —
        // so the client re-sends it here) rather than duplicating that logic.
        var report = await _sender.Send(new GenerateReconciliationReportCommand
        {
            Content = request.Content,
            FileName = request.FileName,
            ContentType = request.ContentType,
            Year = request.Year,
            Month = request.Month,
            PlatformId = request.PlatformId
        }, cancellationToken);

        var isPdf = request.Format == ReportFormat.Pdf;
        var content = isPdf ? _exporter.GeneratePdf(report) : _exporter.GenerateExcel(report);
        var contentType = isPdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = isPdf ? "pdf" : "xlsx";
        var safePlatformName = report.PlatformName.Replace(' ', '-');
        var fileName = $"platform-reconciliation_{safePlatformName}_{request.Year}-{request.Month:D2}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
