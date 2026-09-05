namespace NerjaLogisticsERP.Domain.Events;

public class DailyOrderApprovedEvent : BaseEvent
{
    public DailyOrderApprovedEvent(DailyOrder order) => Order = order;
    public DailyOrder Order { get; }
}
