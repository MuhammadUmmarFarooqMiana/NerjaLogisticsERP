using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.ExportReconciliationReport;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record ExportReconciliationReportCommand : IRequest<DocumentFileResult>
{
    public byte[] Content { get; init; } = [];
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public int Year { get; init; }
    public int Month { get; init; }
    public Guid PlatformId { get; init; }
    public ReportFormat Format { get; init; }
}
