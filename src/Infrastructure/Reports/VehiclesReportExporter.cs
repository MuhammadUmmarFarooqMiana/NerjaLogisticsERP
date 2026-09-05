using ClosedXML.Excel;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Vehicles;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace NerjaLogisticsERP.Infrastructure.Reports;

public class VehiclesReportExporter : IReportExporter<VehiclesReportDto>
{
    private const double MarginLeft = 40;
    private const double MarginRight = 40;
    private const double MarginTop = 40;
    private const double MarginBottom = 40;
    private const double RowHeight = 18;
    private const double ColumnPadding = 6;

    private static readonly (string Header, double Width, bool RightAlign)[] Columns =
    [
        ("Vehicle", 0.15, false),
        ("Record Type", 0.15, false),
        ("Date", 0.13, false),
        ("Description", 0.42, false),
        ("Cost", 0.15, true),
    ];

    public byte[] GeneratePdf(VehiclesReportDto report)
    {
        var titleFont = new XFont("Arial", 18, XFontStyleEx.Bold);
        var periodFont = new XFont("Arial", 10, XFontStyleEx.Regular);
        var summaryFont = new XFont("Arial", 11, XFontStyleEx.Bold);
        var headerFont = new XFont("Arial", 10, XFontStyleEx.Bold);
        var bodyFont = new XFont("Arial", 9, XFontStyleEx.Regular);
        var footerFont = new XFont("Arial", 8, XFontStyleEx.Regular);

        var document = new PdfDocument();
        document.Info.Title = "Vehicles Report";

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

        gfx.DrawString("Vehicles Report", titleFont, XBrushes.Black, new XRect(MarginLeft, y, contentWidth, 26), XStringFormats.TopLeft);
        y += 28;
        gfx.DrawString(report.PeriodLabel, periodFont, XBrushes.Gray, new XRect(MarginLeft, y, contentWidth, 16), XStringFormats.TopLeft);
        y += 26;

        gfx.DrawString(
            $"Total Vehicles: {report.TotalVehicles}    Active: {report.ActiveVehicles}    Total Maintenance Cost: SAR {report.TotalMaintenanceCost:N2}",
            summaryFont, XBrushes.Black, new XRect(MarginLeft, y, contentWidth, 18), XStringFormats.TopLeft);
        y += 28;

        DrawTableHeader();

        if (report.Rows.Count == 0)
        {
            gfx.DrawString("No vehicle activity in this period.", bodyFont, XBrushes.Gray, new XRect(MarginLeft, y, contentWidth, RowHeight), XStringFormats.TopLeft);
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
                row.VehicleRegistration, row.RecordType, row.Date.ToString("yyyy-MM-dd"),
                row.Description ?? "—", row.Cost.HasValue ? $"SAR {row.Cost:N2}" : "—"
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

    public byte[] GenerateExcel(VehiclesReportDto report)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Vehicles Report");

        var row = 1;
        sheet.Cell(row, 1).Value = "Vehicles Report";
        sheet.Cell(row, 1).Style.Font.FontSize = 16;
        sheet.Cell(row, 1).Style.Font.Bold = true;
        row++;

        sheet.Cell(row, 1).Value = report.PeriodLabel;
        sheet.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
        row += 2;

        void SummaryLabel(string label)
        {
            sheet.Cell(row, 1).Value = label;
            sheet.Cell(row, 1).Style.Font.Bold = true;
        }

        SummaryLabel("Total Vehicles");
        sheet.Cell(row, 2).Value = report.TotalVehicles;
        row++;
        SummaryLabel("Active Vehicles");
        sheet.Cell(row, 2).Value = report.ActiveVehicles;
        row++;
        SummaryLabel("Total Maintenance Cost");
        sheet.Cell(row, 2).Value = report.TotalMaintenanceCost;
        row++;
        row++;

        var headerRow = row;
        string[] headers = ["Vehicle", "Record Type", "Date", "Description", "Cost"];
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
            sheet.Cell(row, 1).Value = r.VehicleRegistration;
            sheet.Cell(row, 2).Value = r.RecordType;
            var dateCell = sheet.Cell(row, 3);
            dateCell.Value = r.Date.ToDateTime(TimeOnly.MinValue);
            dateCell.Style.DateFormat.Format = "yyyy-mm-dd";
            sheet.Cell(row, 4).Value = r.Description ?? "—";
            if (r.Cost.HasValue)
                sheet.Cell(row, 5).Value = r.Cost.Value;
            else
                sheet.Cell(row, 5).Value = "—";
            row++;
        }

        sheet.SheetView.FreezeRows(headerRow);
        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
