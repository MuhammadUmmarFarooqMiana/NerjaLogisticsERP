namespace NerjaLogisticsERP.Domain.Events;

public class EmployeeRejectedEvent : BaseEvent
{
    public EmployeeRejectedEvent(Employee employee) => Employee = employee;
    public Employee Employee { get; }
}
