using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Vehicles.Queries.GetVehiclesReport;

namespace NerjaLogisticsERP.Application.Reports.Vehicles.Queries.ExportVehiclesReport;

public class ExportVehiclesReportQueryHandler : IRequestHandler<ExportVehiclesReportQuery, DocumentFileResult>
{
    private readonly ISender _sender;
    private readonly IReportExporter<VehiclesReportDto> _exporter;

    public ExportVehiclesReportQueryHandler(ISender sender, IReportExporter<VehiclesReportDto> exporter)
    {
        _sender = sender;
        _exporter = exporter;
    }

    public async Task<DocumentFileResult> Handle(ExportVehiclesReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetVehiclesReportQuery
        {
            PeriodType = request.PeriodType,
            Date = request.Date,
            Year = request.Year,
            Month = request.Month,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            VehicleId = request.VehicleId
        }, cancellationToken);

        var isPdf = request.Format == ReportFormat.Pdf;
        var content = isPdf ? _exporter.GeneratePdf(report) : _exporter.GenerateExcel(report);
        var contentType = isPdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = isPdf ? "pdf" : "xlsx";
        var fileName = $"vehicles-report_{report.PeriodStart:yyyyMMdd}-{report.PeriodEnd:yyyyMMdd}.{extension}";

        return new DocumentFileResult(content, contentType, fileName);
    }
}
