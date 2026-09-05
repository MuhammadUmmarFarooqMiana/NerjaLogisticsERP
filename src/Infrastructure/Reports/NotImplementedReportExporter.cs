using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Infrastructure.Reports;

/// <summary>
/// Placeholder registered for every <see cref="IReportExporter{TReport}"/> until each report's
/// real PDF/Excel renderer lands (blocked on restoring the PDF/Excel packages in this environment).
/// The JSON preview endpoint each report also exposes works fully without this — only the
/// binary /export route depends on it, and fails loudly rather than silently producing a bad file.
/// </summary>
public class NotImplementedReportExporter<TReport> : IReportExporter<TReport>
{
    public byte[] GeneratePdf(TReport report) =>
        throw new NotSupportedException("PDF export isn't wired up yet for this report.");

    public byte[] GenerateExcel(TReport report) =>
        throw new NotSupportedException("Excel export isn't wired up yet for this report.");
}
