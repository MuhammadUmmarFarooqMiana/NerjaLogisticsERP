using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class DailyOrderRejectedEventHandler : INotificationHandler<DailyOrderRejectedEvent>
{
    private readonly IApplicationDbContext _context;

    public DailyOrderRejectedEventHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DailyOrderRejectedEvent notification, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { notification.Order.EmployeeId }, cancellationToken);
        if (employee is null) return;

        var reason = notification.Order.ReviewNote ?? "No reason provided.";
        var n = Notification.Create(
            employee.UserId,
            "DailyOrderRejected",
            "Daily order rejected",
            $"Your daily order for {notification.Order.OrderDate:yyyy-MM-dd} was rejected: {reason}");
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
