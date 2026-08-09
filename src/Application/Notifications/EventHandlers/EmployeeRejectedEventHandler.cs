using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class EmployeeRejectedEventHandler : INotificationHandler<EmployeeRejectedEvent>
{
    private readonly IApplicationDbContext _context;

    public EmployeeRejectedEventHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(EmployeeRejectedEvent notification, CancellationToken cancellationToken)
    {
        var reason = notification.Employee.RejectionReason ?? "No reason provided.";
        var n = Notification.Create(notification.Employee.UserId, "ProfileRejected",
            "Profile Rejected", $"Your profile was rejected due to the reason: {reason}. Please correct and resubmit for you profile for review. Note: If No reason provided  contact your supervisor");
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
