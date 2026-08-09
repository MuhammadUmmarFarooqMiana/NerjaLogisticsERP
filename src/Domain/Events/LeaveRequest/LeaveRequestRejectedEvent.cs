namespace NerjaLogisticsERP.Domain.Events;
public class LeaveRequestRejectedEvent : BaseEvent
{
    public LeaveRequestRejectedEvent(LeaveRequest request) => Request = request;
    public LeaveRequest Request { get; }
}
