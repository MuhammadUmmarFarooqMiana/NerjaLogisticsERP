namespace NerjaLogisticsERP.Domain.Events;

public class EmployeeProfileSubmittedEvent : BaseEvent
{
    public EmployeeProfileSubmittedEvent(Employee employee) => Employee = employee;
    public Employee Employee { get; }
}
