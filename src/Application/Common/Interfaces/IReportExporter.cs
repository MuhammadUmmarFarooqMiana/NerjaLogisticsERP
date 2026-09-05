namespace NerjaLogisticsERP.Application.Common.Interfaces;

/// <summary>
/// Renders one report's data into a downloadable file. Each report type gets its own
/// implementation (registered against its own <c>TReport</c>) since column layout is
/// inherently specific to what that report shows — only the contract is shared.
/// </summary>
public interface IReportExporter<in TReport>
{
    byte[] GeneratePdf(TReport report);
    byte[] GenerateExcel(TReport report);
}
