namespace NerjaLogisticsERP.Domain;

public class MonthlySummaryVerifiedEvent : BaseEvent
{
    public MonthlySummaryVerifiedEvent(MonthlySummary summary) => Summary = summary;
    public MonthlySummary Summary { get; }
}
