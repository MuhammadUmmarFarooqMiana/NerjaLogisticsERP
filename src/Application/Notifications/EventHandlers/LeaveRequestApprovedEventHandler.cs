using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class LeaveRequestApprovedEventHandler : INotificationHandler<LeaveRequestApprovedEvent>
{
    private readonly IApplicationDbContext _context;

    public LeaveRequestApprovedEventHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(LeaveRequestApprovedEvent notification, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { notification.Request.EmployeeId }, cancellationToken);
        if (employee is null) return;

        var n = Notification.Create(employee.UserId, "LeaveApproved", "Leave Approved",
            $"Your leave request for {notification.Request.StartDate} to {notification.Request.EndDate} was approved.");
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
