using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

// Every Administrator gets notified (they can approve any rider's day), plus
// the rider's own Supervisor specifically — mirrors LeaveRequestSubmittedEventHandler's
// same "all admins + my supervisor" reasoning for a submission awaiting review.
public class DailyOrderClosedEventHandler : INotificationHandler<DailyOrderClosedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public DailyOrderClosedEventHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task Handle(DailyOrderClosedEvent notification, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { notification.Order.EmployeeId }, cancellationToken);
        var employeeName = employee?.FullName ?? "A rider";

        var recipientIds = (await _identityService.GetUserIdsInRoleAsync(Roles.Administrator)).ToHashSet();

        if (employee?.SupervisorId is { } supervisorId)
        {
            var supervisorUserId = await _context.Employees
                .Where(e => e.Id == supervisorId)
                .Select(e => e.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            if (supervisorUserId != Guid.Empty)
                recipientIds.Add(supervisorUserId);
        }

        foreach (var id in recipientIds)
        {
            var n = Notification.Create(id, "DailyOrderAwaitingApproval",
                "Daily Order Awaiting Approval",
                $"{employeeName} closed their daily order for {notification.Order.OrderDate:yyyy-MM-dd} " +
                $"({notification.Order.CompletedOrders} completed orders) — awaiting your approval.");
            _context.Notifications.Add(n);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
