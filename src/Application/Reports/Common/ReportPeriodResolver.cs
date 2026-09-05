using NerjaLogisticsERP.Application.DailyOrders;

namespace NerjaLogisticsERP.Application.Reports.Common;

public readonly record struct ReportPeriod(DateOnly Start, DateOnly End, string Label);

/// <summary>
/// Turns a report's period selection (Daily/Weekly/Monthly/Custom + whichever of
/// Date/Year/Month/StartDate/EndDate applies) into a concrete date range every
/// report type resolves the same way, so period semantics never drift between them.
/// "Today" is KSA-local, matching DailyOrderClock — the same calendar day a
/// rider's daily order resets on, not the UTC day.
/// </summary>
public static class ReportPeriodResolver
{
    public static ReportPeriod Resolve(
        ReportPeriodType periodType,
        DateOnly? date,
        int? year,
        int? month,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        var today = DailyOrderClock.Today();

        switch (periodType)
        {
            case ReportPeriodType.Daily:
                {
                    var d = date ?? today;
                    return new ReportPeriod(d, d, $"Daily — {d:yyyy-MM-dd}");
                }
            case ReportPeriodType.Weekly:
                {
                    var d = date ?? today;
                    var daysSinceMonday = ((int)d.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                    var start = d.AddDays(-daysSinceMonday);
                    var end = start.AddDays(6);
                    return new ReportPeriod(start, end, $"Weekly — {start:yyyy-MM-dd} to {end:yyyy-MM-dd}");
                }
            case ReportPeriodType.Monthly:
                {
                    var y = year ?? today.Year;
                    var m = month ?? today.Month;
                    var start = new DateOnly(y, m, 1);
                    var end = start.AddMonths(1).AddDays(-1);
                    return new ReportPeriod(start, end, $"Monthly — {start:yyyy-MM}");
                }
            case ReportPeriodType.Custom:
                {
                    // Presence/ordering of StartDate/EndDate is enforced by each query's validator
                    // before the handler ever reaches this call.
                    var s = startDate!.Value <= endDate!.Value ? startDate.Value : endDate.Value;
                    var e = startDate.Value <= endDate.Value ? endDate.Value : startDate.Value;
                    return new ReportPeriod(s, e, $"Custom — {s:yyyy-MM-dd} to {e:yyyy-MM-dd}");
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(periodType), periodType, null);
        }
    }
}
