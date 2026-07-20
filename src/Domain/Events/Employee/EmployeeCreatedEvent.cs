namespace NerjaLogisticsERP.Domain.Events;

public class EmployeeCreatedEvent : BaseEvent
{
    public EmployeeCreatedEvent(Employee employee) => Employee = employee;
    public Employee Employee { get; }
}
