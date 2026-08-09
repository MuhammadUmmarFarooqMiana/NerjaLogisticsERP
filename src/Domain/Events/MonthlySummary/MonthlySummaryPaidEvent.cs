namespace NerjaLogisticsERP.Domain.Events;

public class MonthlySummaryPaidEvent : BaseEvent
{
    public MonthlySummaryPaidEvent(MonthlySummary summary) => Summary = summary;
    public MonthlySummary Summary { get; }
}
