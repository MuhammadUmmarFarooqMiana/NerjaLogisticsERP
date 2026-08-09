namespace NerjaLogisticsERP.Domain.Events;

public class DailyOrderClosedEvent : BaseEvent
{
    public DailyOrderClosedEvent(DailyOrder order) => Order = order;
    public DailyOrder Order { get; }
}
