namespace NerjaLogisticsERP.Domain.Events;

public class DailyOrderRejectedEvent : BaseEvent
{
    public DailyOrderRejectedEvent(DailyOrder order) => Order = order;
    public DailyOrder Order { get; }
}
