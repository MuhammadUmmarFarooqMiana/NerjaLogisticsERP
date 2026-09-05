namespace NerjaLogisticsERP.Domain.Events;

// Fired when a Supervisor/Administrator edits a Rejected order's count and
// finalizes it as Approved — distinct from DailyOrderApprovedEvent so the
// rider's notification can explain the count actually changed, not just that
// the originally-submitted number was approved as-is.
public class DailyOrderCorrectedEvent : BaseEvent
{
    public DailyOrderCorrectedEvent(DailyOrder order) => Order = order;
    public DailyOrder Order { get; }
}
