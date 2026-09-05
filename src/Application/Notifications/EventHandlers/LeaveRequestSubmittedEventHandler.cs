using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class LeaveRequestSubmittedEventHandler : INotificationHandler<LeaveRequestSubmittedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public LeaveRequestSubmittedEventHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task Handle(LeaveRequestSubmittedEvent notification, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { notification.Request.EmployeeId }, cancellationToken);
        var employeeName = employee?.FullName ?? "An employee";

        // Every Administrator gets notified (they can review any request), plus the
        // submitter's own Supervisor specifically — not every Supervisor role-wide,
        // since only their own reports' requests will show up in that Supervisor's queue.
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
            var n = Notification.Create(id, "LeaveRequestSubmitted",
                "New Leave Request",
                $"{employeeName} submitted a leave request for {notification.Request.StartDate} to {notification.Request.EndDate}.");
            _context.Notifications.Add(n);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
