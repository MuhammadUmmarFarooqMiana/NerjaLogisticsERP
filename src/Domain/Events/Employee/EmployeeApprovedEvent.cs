namespace NerjaLogisticsERP.Domain.Events;


public class EmployeeApprovedEvent : BaseEvent
{
    public EmployeeApprovedEvent(Employee employee) => Employee = employee;
    public Employee Employee { get; }
}
