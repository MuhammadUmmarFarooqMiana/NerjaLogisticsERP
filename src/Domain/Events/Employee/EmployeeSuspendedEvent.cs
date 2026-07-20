namespace NerjaLogisticsERP.Domain.Events;

public class EmployeeSuspendedEvent : BaseEvent
{
    public EmployeeSuspendedEvent(Employee employee) => Employee = employee;
    public Employee Employee { get; }
}
