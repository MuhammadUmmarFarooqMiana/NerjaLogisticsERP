namespace NerjaLogisticsERP.Domain.Events;

public class DailyOrderStartedEvent : BaseEvent
{
    public DailyOrderStartedEvent(DailyOrder order) => Order = order;
    public DailyOrder Order { get; }
}
