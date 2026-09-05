using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class MonthlySummaryPaidEventHandler : INotificationHandler<MonthlySummaryPaidEvent>
{
    private readonly IApplicationDbContext _context;

    public MonthlySummaryPaidEventHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(MonthlySummaryPaidEvent notification, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { notification.Summary.EmployeeId }, cancellationToken);
        if (employee is null) return;

        var n = Notification.Create(employee.UserId, "SalaryReady", "Salary Paid",
            $"Your salary for {notification.Summary.Month}/{notification.Summary.Year} ({notification.Summary.NetSalaryPayable:F2} SAR) has been paid.");
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
