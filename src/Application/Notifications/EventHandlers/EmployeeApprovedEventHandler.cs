using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class EmployeeApprovedEventHandler : INotificationHandler<EmployeeApprovedEvent>
{
    private readonly IApplicationDbContext _context;

    public EmployeeApprovedEventHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(EmployeeApprovedEvent notification, CancellationToken cancellationToken)
    {
        var _notification = Notification.Create(notification.Employee.UserId, "AccountApproved",
            "Account Approved", "Your account has been approved. You can now log in.");
        _context.Notifications.Add(_notification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
