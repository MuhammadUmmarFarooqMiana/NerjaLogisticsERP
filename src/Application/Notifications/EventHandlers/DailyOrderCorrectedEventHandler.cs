using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class DailyOrderCorrectedEventHandler : INotificationHandler<DailyOrderCorrectedEvent>
{
    private readonly IApplicationDbContext _context;

    public DailyOrderCorrectedEventHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DailyOrderCorrectedEvent notification, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { notification.Order.EmployeeId }, cancellationToken);
        if (employee is null) return;

        var reason = notification.Order.ReviewNote ?? "No reason provided.";
        var n = Notification.Create(
            employee.UserId,
            "DailyOrderCorrected",
            "Daily order corrected",
            $"Your daily order for {notification.Order.OrderDate:yyyy-MM-dd} was corrected and approved. " +
            $"New count: {notification.Order.CompletedOrders}. Reason: {reason}");
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
