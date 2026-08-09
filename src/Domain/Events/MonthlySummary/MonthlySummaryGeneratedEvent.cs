namespace NerjaLogisticsERP.Domain.Events;

public class MonthlySummaryGeneratedEvent : BaseEvent
{
    public MonthlySummaryGeneratedEvent(MonthlySummary summary) => Summary = summary;
    public MonthlySummary Summary { get; }
}
