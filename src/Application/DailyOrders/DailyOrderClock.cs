namespace NerjaLogisticsERP.Application.DailyOrders;

// Daily orders reset on KSA-local days, not UTC days — shared by every
// command/query here (and by Infrastructure's auto-close background job)
// so "today" always means the same thing.
public static class DailyOrderClock
{
    public static readonly TimeZoneInfo KsaZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");

    public static DateOnly Today() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, KsaZone).Date);
}
