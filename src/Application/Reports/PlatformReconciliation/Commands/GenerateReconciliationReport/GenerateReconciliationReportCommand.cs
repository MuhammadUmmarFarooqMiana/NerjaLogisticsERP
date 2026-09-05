using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.GenerateReconciliationReport;

/// <summary>
/// Cross-verifies a platform-provided (Keeta/Hunger/Jahez) payout workbook against Nerja's
/// own MonthlySummary records for the same period — see PlatformReconciliationReportDto for
/// what "matched" / "unmatched" / "missing" mean. Modeled as a Command (not a Query) purely
/// because it carries an uploaded file, matching this codebase's Upload* convention — nothing
/// is persisted; the same request re-run with the same file always yields the same result.
/// </summary>
[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GenerateReconciliationReportCommand : IRequest<PlatformReconciliationReportDto>
{
    public byte[] Content { get; init; } = [];
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public int Year { get; init; }
    public int Month { get; init; }
    public Guid PlatformId { get; init; }
}
