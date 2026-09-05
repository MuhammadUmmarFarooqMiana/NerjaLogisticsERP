using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class DailyOrderApprovedEventHandler : INotificationHandler<DailyOrderApprovedEvent>
{
    private readonly IApplicationDbContext _context;

    public DailyOrderApprovedEventHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DailyOrderApprovedEvent notification, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { notification.Order.EmployeeId }, cancellationToken);
        if (employee is null) return;

        var n = Notification.Create(
            employee.UserId,
            "DailyOrderApproved",
            "Daily order approved",
            $"Your daily order for {notification.Order.OrderDate:yyyy-MM-dd} was approved: {notification.Order.CompletedOrders} completed orders.");
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
