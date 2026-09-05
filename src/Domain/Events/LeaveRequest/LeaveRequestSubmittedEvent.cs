namespace NerjaLogisticsERP.Domain.Events;

public class LeaveRequestSubmittedEvent : BaseEvent
{
    public LeaveRequestSubmittedEvent(LeaveRequest request) => Request = request;
    public LeaveRequest Request { get; }
}
