namespace NerjaLogisticsERP.Domain.Events;

public class LeaveRequestApprovedEvent : BaseEvent
{
    public LeaveRequestApprovedEvent(LeaveRequest request) => Request = request;
    public LeaveRequest Request { get; }
}
