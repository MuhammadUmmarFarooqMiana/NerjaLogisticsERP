using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Reports.Vehicles.Queries.ExportVehiclesReport;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record ExportVehiclesReportQuery : IRequest<DocumentFileResult>
{
    public ReportPeriodType PeriodType { get; init; }
    public ReportFormat Format { get; init; }
    public DateOnly? Date { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public Guid? VehicleId { get; init; }
}
