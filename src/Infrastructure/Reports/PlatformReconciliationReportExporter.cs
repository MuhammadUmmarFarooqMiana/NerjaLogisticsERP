using ClosedXML.Excel;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.PlatformReconciliation;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace NerjaLogisticsERP.Infrastructure.Reports;

public class PlatformReconciliationReportExporter : IReportExporter<PlatformReconciliationReportDto>
{
    private const double MarginLeft = 40;
    private const double MarginRight = 40;
    private const double MarginTop = 40;
    private const double MarginBottom = 40;
    private const double RowHeight = 16;
    private const double ColumnPadding = 5;

    private static readonly (string Header, double Width, bool RightAlign)[] MatchedColumns =
    [
        ("Employee", 0.20, false),
        ("Platform ID", 0.10, false),
        ("Nerja Orders", 0.09, true),
        ("Platform Orders", 0.10, true),
        ("Diff", 0.07, true),
        ("Penalties", 0.11, true),
        ("Original Net", 0.11, true),
        ("Adjusted Net", 0.11, true),
        ("Status", 0.11, false),
    ];

    private static readonly (string Header, double Width, bool RightAlign)[] UnmatchedColumns =
    [
        ("Platform ID", 0.34, false),
        ("Platform Orders", 0.33, true),
        ("Penalties", 0.33, true),
    ];

    private static readonly (string Header, double Width, bool RightAlign)[] MissingColumns =
    [
        ("Employee", 0.34, false),
        ("Platform ID", 0.22, false),
        ("Nerja Orders", 0.22, true),
        ("Net Payable", 0.22, true),
    ];

    public byte[] GeneratePdf(PlatformReconciliationReportDto report)
    {
        var titleFont = new XFont("Arial", 18, XFontStyleEx.Bold);
        var periodFont = new XFont("Arial", 10, XFontStyleEx.Regular);
        var summaryFont = new XFont("Arial", 10, XFontStyleEx.Bold);
        var sectionFont = new XFont("Arial", 11, XFontStyleEx.Bold);
        var headerFont = new XFont("Arial", 8, XFontStyleEx.Bold);
        var bodyFont = new XFont("Arial", 8, XFontStyleEx.Regular);
        var noteFont = new XFont("Arial", 8, XFontStyleEx.Italic);
        var footerFont = new XFont("Arial", 8, XFontStyleEx.Regular);

        var document = new PdfDocument();
        document.Info.Title = "Platform Reconciliation Report";

        var page = document.AddPage();
        page.Size = PdfSharp.PageSize.A4;
        var gfx = XGraphics.FromPdfPage(page);
        var contentWidth = page.Width.Point - MarginLeft - MarginRight;
        var pageBottom = page.Height.Point - MarginBottom;
        var y = MarginTop;
        var pageNumber = 1;

        void DrawFooter()
        {
            gfx.DrawString($"Page {pageNumber}", footerFont, XBrushes.Gray,
                new XRect(MarginLeft, page.Height.Point - 24, contentWidth, 16), XStringFormats.TopRight);
        }

        void StartNewPage()
        {
            DrawFooter();
            page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            gfx = XGraphics.FromPdfPage(page);
            y = MarginTop;
            pageNumber++;
        }

        void EnsureSpace(double needed)
        {
            if (y + needed > pageBottom) StartNewPage();
        }

        void DrawRow((string Header, double Width, bool RightAlign)[] columns, string[] values, XFont font, XBrush brush)
        {
            var x = MarginLeft;
            for (var i = 0; i < columns.Length; i++)
            {
                var width = contentWidth * columns[i].Width;
                gfx.DrawString(values[i], font, brush, new XRect(x, y, width - ColumnPadding, RowHeight),
                    columns[i].RightAlign ? XStringFormats.TopRight : XStringFormats.TopLeft);
                x += width;
            }
            y += RowHeight;
        }

        void DrawSection(string title, (string Header, double Width, bool RightAlign)[] columns, IReadOnlyList<string[]> rows, string emptyMessage)
        {
            EnsureSpace(RowHeight * 3);
            gfx.DrawString(title, sectionFont, XBrushes.Black, new XRect(MarginLeft, y, contentWidth, RowHeight + 4), XStringFormats.TopLeft);
            y += RowHeight + 8;

            void DrawHeader()
            {
                DrawRow(columns, columns.Select(c => c.Header).ToArray(), headerFont, XBrushes.Black);
                gfx.DrawLine(XPens.Black, MarginLeft, y, MarginLeft + contentWidth, y);
                y += 3;
            }

            DrawHeader();

            if (rows.Count == 0)
            {
                gfx.DrawString(emptyMessage, noteFont, XBrushes.Gray, new XRect(MarginLeft, y, contentWidth, RowHeight), XStringFormats.TopLeft);
                y += RowHeight;
            }

            foreach (var row in rows)
            {
                EnsureSpace(RowHeight);
                // A page break mid-section needs the header redrawn so the new page is still legible on its own.
                if (y == MarginTop) DrawHeader();
                DrawRow(columns, row, bodyFont, XBrushes.Black);
            }

            y += 14;
        }

        gfx.DrawString("Platform Reconciliation Report", titleFont, XBrushes.Black, new XRect(MarginLeft, y, contentWidth, 26), XStringFormats.TopLeft);
        y += 28;
        gfx.DrawString($"{report.PlatformName} — {report.PeriodLabel}", periodFont, XBrushes.Gray, new XRect(MarginLeft, y, contentWidth, 16), XStringFormats.TopLeft);
        y += 24;

        gfx.DrawString(
            $"Matched: {report.TotalMatched}    Order mismatches: {report.TotalOrdersMismatchCount}    " +
            $"Unmatched in sheet: {report.TotalUnmatchedInPlatform}    Missing from sheet: {report.TotalMissingFromSheet}",
            summaryFont, XBrushes.Black, new XRect(MarginLeft, y, contentWidth, 16), XStringFormats.TopLeft);
        y += 18;
        gfx.DrawString(
            $"Total penalties: SAR {report.TotalPenalties:N2}    Original net payable: SAR {report.TotalOriginalNetPayable:N2}    " +
            $"Adjusted net payable: SAR {report.TotalAdjustedNetPayable:N2}",
            summaryFont, XBrushes.Black, new XRect(MarginLeft, y, contentWidth, 16), XStringFormats.TopLeft);
        y += 26;

        DrawSection(
            "Matched Riders",
            MatchedColumns,
            report.MatchedRows.Select(r => new[]
            {
                r.EmployeeName, r.PlatformIdNumber, r.NerjaCompletedOrders.ToString(), r.PlatformCompletedOrders.ToString(),
                r.OrdersDifference.ToString("+#;-#;0"), $"SAR {r.TotalPenalties:N2}", $"SAR {r.OriginalNetPayable:N2}",
                $"SAR {r.AdjustedNetPayable:N2}", r.MonthlySummaryStatus,
            }).ToList(),
            "No riders matched.");

        DrawSection(
            "In Platform Sheet, No Matching Employee",
            UnmatchedColumns,
            report.UnmatchedPlatformRows.Select(r => new[] { r.PlatformIdNumber, r.PlatformCompletedOrders.ToString(), $"SAR {r.TotalPenalties:N2}" }).ToList(),
            "None — every rider_id in the sheet matched a Nerja employee.");

        DrawSection(
            "Nerja Employees Missing From Sheet",
            MissingColumns,
            report.MissingFromSheetRows.Select(r => new[]
            {
                r.EmployeeName, r.PlatformIdNumber ?? "—", r.NerjaCompletedOrders.ToString(), $"SAR {r.NetPayable:N2}",
            }).ToList(),
            "None — every Nerja employee on this platform appears in the sheet.");

        DrawFooter();

        using var stream = new MemoryStream();
        document.Save(stream, closeStream: false);
        return stream.ToArray();
    }

    public byte[] GenerateExcel(PlatformReconciliationReportDto report)
    {
        using var workbook = new XLWorkbook();

        var summarySheet = workbook.Worksheets.Add("Summary");
        var row = 1;
        summarySheet.Cell(row, 1).Value = "Platform Reconciliation Report";
        summarySheet.Cell(row, 1).Style.Font.FontSize = 16;
        summarySheet.Cell(row, 1).Style.Font.Bold = true;
        row++;
        summarySheet.Cell(row, 1).Value = $"{report.PlatformName} — {report.PeriodLabel}";
        summarySheet.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
        row += 2;

        void SummaryRow(string label, object value)
        {
            summarySheet.Cell(row, 1).Value = label;
            summarySheet.Cell(row, 1).Style.Font.Bold = true;
            summarySheet.Cell(row, 2).Value = XLCellValue.FromObject(value);
            row++;
        }

        SummaryRow("Matched Riders", report.TotalMatched);
        SummaryRow("Order Mismatches", report.TotalOrdersMismatchCount);
        SummaryRow("Unmatched In Platform Sheet", report.TotalUnmatchedInPlatform);
        SummaryRow("Missing From Sheet", report.TotalMissingFromSheet);
        SummaryRow("Total Penalties", report.TotalPenalties);
        SummaryRow("Total Original Net Payable", report.TotalOriginalNetPayable);
        SummaryRow("Total Adjusted Net Payable", report.TotalAdjustedNetPayable);
        summarySheet.Columns().AdjustToContents();

        var matchedSheet = workbook.Worksheets.Add("Matched");
        string[] matchedHeaders =
        [
            "Employee", "Platform ID", "Nerja Orders", "Platform Orders", "Orders Difference",
            "Stacking Deduction", "Declined Penalties", "Late Penalty", "No-Show Penalty",
            "No-Show Penalty (Special Cities)", "Daily Acceptance Rate Penalty", "Missed Days Penalty",
            "Total Penalties", "Original Net Payable", "Adjusted Net Payable", "Status",
        ];
        WriteHeader(matchedSheet, matchedHeaders);
        var r = 2;
        foreach (var item in report.MatchedRows)
        {
            matchedSheet.Cell(r, 1).Value = item.EmployeeName;
            matchedSheet.Cell(r, 2).Value = item.PlatformIdNumber;
            matchedSheet.Cell(r, 3).Value = item.NerjaCompletedOrders;
            matchedSheet.Cell(r, 4).Value = item.PlatformCompletedOrders;
            matchedSheet.Cell(r, 5).Value = item.OrdersDifference;
            matchedSheet.Cell(r, 6).Value = item.StackingDeduction;
            matchedSheet.Cell(r, 7).Value = item.DeclinedPenaltiesDayLogic;
            matchedSheet.Cell(r, 8).Value = item.LatePenalty;
            matchedSheet.Cell(r, 9).Value = item.NoShowPenalty;
            matchedSheet.Cell(r, 10).Value = item.NoShowPenaltySpecialCities;
            matchedSheet.Cell(r, 11).Value = item.DailyAcceptanceRatePenalty;
            matchedSheet.Cell(r, 12).Value = item.MissedDaysPenalty;
            matchedSheet.Cell(r, 13).Value = item.TotalPenalties;
            matchedSheet.Cell(r, 14).Value = item.OriginalNetPayable;
            matchedSheet.Cell(r, 15).Value = item.AdjustedNetPayable;
            matchedSheet.Cell(r, 16).Value = item.MonthlySummaryStatus;
            r++;
        }
        matchedSheet.SheetView.FreezeRows(1);
        matchedSheet.Columns().AdjustToContents();

        var unmatchedSheet = workbook.Worksheets.Add("Unmatched In Sheet");
        WriteHeader(unmatchedSheet, ["Platform ID", "Platform Orders", "Total Penalties"]);
        r = 2;
        foreach (var item in report.UnmatchedPlatformRows)
        {
            unmatchedSheet.Cell(r, 1).Value = item.PlatformIdNumber;
            unmatchedSheet.Cell(r, 2).Value = item.PlatformCompletedOrders;
            unmatchedSheet.Cell(r, 3).Value = item.TotalPenalties;
            r++;
        }
        unmatchedSheet.SheetView.FreezeRows(1);
        unmatchedSheet.Columns().AdjustToContents();

        var missingSheet = workbook.Worksheets.Add("Missing From Sheet");
        WriteHeader(missingSheet, ["Employee", "Platform ID", "Nerja Orders", "Net Payable"]);
        r = 2;
        foreach (var item in report.MissingFromSheetRows)
        {
            missingSheet.Cell(r, 1).Value = item.EmployeeName;
            missingSheet.Cell(r, 2).Value = item.PlatformIdNumber ?? string.Empty;
            missingSheet.Cell(r, 3).Value = item.NerjaCompletedOrders;
            missingSheet.Cell(r, 4).Value = item.NetPayable;
            r++;
        }
        missingSheet.SheetView.FreezeRows(1);
        missingSheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void WriteHeader(IXLWorksheet sheet, string[] headers)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var cell = sheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }
    }
}
