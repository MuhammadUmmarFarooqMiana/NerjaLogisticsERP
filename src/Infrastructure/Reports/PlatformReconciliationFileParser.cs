using System.Globalization;
using ClosedXML.Excel;
using FluentValidation.Results;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Infrastructure.Reports;

/// <summary>
/// Reads the "Rider LVL" sheet from a Keeta/Hunger/Jahez payout workbook. Each header cell in
/// that sheet is formatted "&lt;arabic label&gt; - &lt;english_key&gt;" (e.g.
/// "الطلبات المكتملة - completed_orders") — this locates columns by the english_key suffix
/// rather than by fixed position, so it keeps working if the provider reorders columns
/// between exports (they're a third party; we don't control their file layout).
/// </summary>
public class PlatformReconciliationFileParser : IPlatformReconciliationFileParser
{
    private const string SheetName = "Rirder LVL";

    private static readonly string[] RequiredKeys =
    [
        "rider_id",
        "completed_orders",
        "stacking_deduction",
        "declined_penalties_day_logic",
        "late_penalty",
        "no_show_penalty",
        "no_show_penalty_special_cities",
        "daily_acceptance_rate_penalty",
        "missed_days_penalty",
    ];

    public List<PlatformRiderRow> ParseRiderLevelSheet(byte[] content)
    {
        IXLWorksheet sheet;
        try
        {
            using var stream = new MemoryStream(content);
            using var workbook = new XLWorkbook(stream);

            sheet = workbook.Worksheets.FirstOrDefault(
                w => string.Equals(w.Name.Trim(), SheetName, StringComparison.OrdinalIgnoreCase))
                ?? throw Fail($"The workbook must contain a sheet named \"{SheetName}\" (found: {string.Join(", ", workbook.Worksheets.Select(w => w.Name))}).");

            return ParseSheet(sheet);
        }
        catch (Exception ex) when (ex is not ValidationException)
        {
            throw Fail("The uploaded file isn't a readable Excel workbook.");
        }
    }

    private static List<PlatformRiderRow> ParseSheet(IXLWorksheet sheet)
    {
        var headerRow = sheet.Row(1);
        var lastHeaderColumn = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;

        var columnByKey = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var col = 1; col <= lastHeaderColumn; col++)
        {
            var key = ExtractKey(headerRow.Cell(col).GetString());
            if (!string.IsNullOrEmpty(key) && !columnByKey.ContainsKey(key))
                columnByKey[key] = col;
        }

        var missing = RequiredKeys.Where(k => !columnByKey.ContainsKey(k)).ToList();
        if (missing.Count > 0)
        {
            throw Fail($"The \"{SheetName}\" sheet is missing expected column(s): {string.Join(", ", missing)}.");
        }

        var rows = new List<PlatformRiderRow>();
        var lastDataRow = sheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var r = 2; r <= lastDataRow; r++)
        {
            var row = sheet.Row(r);
            var riderIdCell = row.Cell(columnByKey["rider_id"]);
            if (riderIdCell.IsEmpty()) continue; // blank trailing row

            rows.Add(new PlatformRiderRow
            {
                RiderId = ReadId(riderIdCell),
                CompletedOrders = (int)ReadDecimal(row.Cell(columnByKey["completed_orders"])),
                StackingDeduction = ReadDecimal(row.Cell(columnByKey["stacking_deduction"])),
                DeclinedPenaltiesDayLogic = ReadDecimal(row.Cell(columnByKey["declined_penalties_day_logic"])),
                LatePenalty = ReadDecimal(row.Cell(columnByKey["late_penalty"])),
                NoShowPenalty = ReadDecimal(row.Cell(columnByKey["no_show_penalty"])),
                NoShowPenaltySpecialCities = ReadDecimal(row.Cell(columnByKey["no_show_penalty_special_cities"])),
                DailyAcceptanceRatePenalty = ReadDecimal(row.Cell(columnByKey["daily_acceptance_rate_penalty"])),
                MissedDaysPenalty = ReadDecimal(row.Cell(columnByKey["missed_days_penalty"])),
            });
        }

        return rows;
    }

    /// <summary>"معرف المندوب - rider_id" -&gt; "rider_id". A header with no " - " separator
    /// (e.g. "Contract Name") has no english_key and is simply not indexed — those columns
    /// aren't needed for this report.</summary>
    private static string ExtractKey(string header)
    {
        var trimmed = header.Trim();
        var idx = trimmed.LastIndexOf(" - ", StringComparison.Ordinal);
        return idx < 0 ? string.Empty : trimmed[(idx + 3)..].Trim();
    }

    /// <summary>rider_id is a plain integer in every sample seen, but read defensively:
    /// numeric cells are formatted without a decimal point or thousands separator so
    /// "1417658.0" never leaks into the match key, and a text-typed id is used as-is.</summary>
    private static string ReadId(IXLCell cell) =>
        cell.DataType == XLDataType.Number
            ? cell.GetDouble().ToString("0", CultureInfo.InvariantCulture)
            : cell.GetString().Trim();

    private static decimal ReadDecimal(IXLCell cell)
    {
        if (cell.IsEmpty()) return 0m;
        if (cell.DataType == XLDataType.Number) return (decimal)cell.GetDouble();
        return decimal.TryParse(cell.GetString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0m;
    }

    private static ValidationException Fail(string message) =>
        new([new ValidationFailure("File", message)]);
}
