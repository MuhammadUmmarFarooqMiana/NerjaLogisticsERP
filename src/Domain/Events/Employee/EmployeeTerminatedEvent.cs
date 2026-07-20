namespace NerjaLogisticsERP.Domain.Events;

public class EmployeeTerminatedEvent : BaseEvent
{
    public EmployeeTerminatedEvent(Employee employee) => Employee = employee;
    public Employee Employee { get; }
}
