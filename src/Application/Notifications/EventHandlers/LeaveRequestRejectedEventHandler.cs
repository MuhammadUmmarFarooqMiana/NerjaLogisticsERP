using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class LeaveRequestRejectedEventHandler : INotificationHandler<LeaveRequestRejectedEvent>
{
    private readonly IApplicationDbContext _context;

    public LeaveRequestRejectedEventHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(LeaveRequestRejectedEvent notification, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { notification.Request.EmployeeId }, cancellationToken);
        if (employee is null) return;

        var reason = notification.Request.RejectionReason ?? "No reason provided.";
        var n = Notification.Create(employee.UserId, "LeaveRejected", "Leave Rejected", $"Your leave request was rejected: {reason}");
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
