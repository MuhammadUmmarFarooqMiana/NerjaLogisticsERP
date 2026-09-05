using ClosedXML.Excel;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Salaries;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace NerjaLogisticsERP.Infrastructure.Reports;

public class SalariesReportExporter : IReportExporter<SalariesReportDto>
{
    private const double MarginLeft = 40;
    private const double MarginRight = 40;
    private const double MarginTop = 40;
    private const double MarginBottom = 40;
    private const double RowHeight = 18;
    private const double ColumnPadding = 6;

    private static readonly (string Header, double Width, bool RightAlign)[] Columns =
    [
        ("Employee", 0.19, false),
        ("Platform ID", 0.13, false),
        ("Period", 0.10, false),
        ("Salary", 0.13, true),
        ("Advances", 0.12, true),
        ("Fines", 0.11, true),
        ("Net Payable", 0.13, true),
        ("Status", 0.09, false),
    ];

    public byte[] GeneratePdf(SalariesReportDto report)
    {
        var titleFont = new XFont("Arial", 18, XFontStyleEx.Bold);
        var periodFont = new XFont("Arial", 10, XFontStyleEx.Regular);
        var summaryFont = new XFont("Arial", 11, XFontStyleEx.Bold);
        var headerFont = new XFont("Arial", 9, XFontStyleEx.Bold);
        var bodyFont = new XFont("Arial", 9, XFontStyleEx.Regular);
        var footerFont = new XFont("Arial", 8, XFontStyleEx.Regular);

        var document = new PdfDocument();
        document.Info.Title = "Salaries Report";

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

        void DrawTableHeader()
        {
            var x = MarginLeft;
            foreach (var column in Columns)
            {
                var width = contentWidth * column.Width;
                gfx.DrawString(column.Header, headerFont, XBrushes.Black, new XRect(x, y, width - ColumnPadding, RowHeight),
                    column.RightAlign ? XStringFormats.TopRight : XStringFormats.TopLeft);
                x += width;
            }
            y += RowHeight;
            gfx.DrawLine(XPens.Black, MarginLeft, y, MarginLeft + contentWidth, y);
            y += 4;
        }

        gfx.DrawString("Salaries Report", titleFont, XBrushes.Black, new XRect(MarginLeft, y, contentWidth, 26), XStringFormats.TopLeft);
        y += 28;
        gfx.DrawString(report.PeriodLabel, periodFont, XBrushes.Gray, new XRect(MarginLeft, y, contentWidth, 16), XStringFormats.TopLeft);
        y += 26;

        gfx.DrawString(
            $"Total Salary: SAR {report.TotalSalary:N2}    Advances: SAR {report.TotalAdvances:N2}    Fines: SAR {report.TotalFines:N2}    Net Payable: SAR {report.TotalNetPayable:N2}",
            summaryFont, XBrushes.Black, new XRect(MarginLeft, y, contentWidth, 18), XStringFormats.TopLeft);
        y += 28;

        DrawTableHeader();

        if (report.Rows.Count == 0)
        {
            gfx.DrawString("No salary summaries in this period.", bodyFont, XBrushes.Gray, new XRect(MarginLeft, y, contentWidth, RowHeight), XStringFormats.TopLeft);
            y += RowHeight;
        }

        foreach (var row in report.Rows)
        {
            if (y + RowHeight > pageBottom)
            {
                StartNewPage();
                DrawTableHeader();
            }

            var x = MarginLeft;
            var values = new[]
            {
                row.EmployeeName, row.PlatformIdNumber ?? "—", $"{row.Month}/{row.Year}", $"SAR {row.TotalSalary:N2}",
                $"SAR {row.TotalAdvances:N2}", $"SAR {row.TotalFines:N2}", $"SAR {row.NetSalaryPayable:N2}", row.Status
            };
            for (var i = 0; i < Columns.Length; i++)
            {
                var width = contentWidth * Columns[i].Width;
                gfx.DrawString(values[i], bodyFont, XBrushes.Black, new XRect(x, y, width - ColumnPadding, RowHeight),
                    Columns[i].RightAlign ? XStringFormats.TopRight : XStringFormats.TopLeft);
                x += width;
            }
            y += RowHeight;
        }

        DrawFooter();

        using var stream = new MemoryStream();
        document.Save(stream, closeStream: false);
        return stream.ToArray();
    }

    public byte[] GenerateExcel(SalariesReportDto report)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Salaries Report");

        var row = 1;
        sheet.Cell(row, 1).Value = "Salaries Report";
        sheet.Cell(row, 1).Style.Font.FontSize = 16;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        row++;

        sheet.Cell(row, 1).Value = report.PeriodLabel;
        sheet.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
        row += 2;

        void SummaryRow(string label, decimal value)
        {
            sheet.Cell(row, 1).Value = label;
            sheet.Cell(row, 1).Style.Font.Bold = true;
            sheet.Cell(row, 2).Value = value;
            row++;
        }

        SummaryRow("Total Salary", report.TotalSalary);
        SummaryRow("Total Advances", report.TotalAdvances);
        SummaryRow("Total Fines", report.TotalFines);
        SummaryRow("Total Net Payable", report.TotalNetPayable);
        row++;

        var headerRow = row;
        string[] headers = ["Employee", "Platform ID", "Year", "Month", "Orders", "Salary", "Advances", "Fines", "Net Payable", "Status"];
        for (var i = 0; i < headers.Length; i++)
        {
            var cell = sheet.Cell(headerRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }
        row++;

        foreach (var r in report.Rows)
        {
            sheet.Cell(row, 1).Value = r.EmployeeName;
            sheet.Cell(row, 2).Value = r.PlatformIdNumber ?? string.Empty;
            sheet.Cell(row, 3).Value = r.Year;
            sheet.Cell(row, 4).Value = r.Month;
            sheet.Cell(row, 5).Value = r.TotalCompletedOrders;
            sheet.Cell(row, 6).Value = r.TotalSalary;
            sheet.Cell(row, 7).Value = r.TotalAdvances;
            sheet.Cell(row, 8).Value = r.TotalFines;
            sheet.Cell(row, 9).Value = r.NetSalaryPayable;
            sheet.Cell(row, 10).Value = r.Status;
            row++;
        }

        sheet.SheetView.FreezeRows(headerRow);
        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
