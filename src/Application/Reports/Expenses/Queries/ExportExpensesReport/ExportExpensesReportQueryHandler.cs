using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Expenses.Queries.GetExpensesReport;

namespace NerjaLogisticsERP.Application.Reports.Expenses.Queries.ExportExpensesReport;

public class ExportExpensesReportQueryHandler : IRequestHandler<ExportExpensesReportQuery, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<ExpensesReportDto> _exporter;

    public ExportExpensesReportQueryHandler(ISender sender, IReportExporter<ExpensesReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportExpensesReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetExpensesReportQuery
        {
            PeriodType = request.PeriodType,
            Date = request.Date,
            Year = request.Year,
            Month = request.Month,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Category = request.Category,
            PlatformId = request.PlatformId
        }, cancellationToken);

        var isPdf = request.Format == ReportFormat.Pdf;
        var content = isPdf ? _exporter.GeneratePdf(report) : _exporter.GenerateExcel(report);
        var contentType = isPdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = isPdf ? "pdf" : "xlsx";
        var fileName = $"expenses-report_{report.PeriodStart:yyyyMMdd}-{report.PeriodEnd:yyyyMMdd}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
