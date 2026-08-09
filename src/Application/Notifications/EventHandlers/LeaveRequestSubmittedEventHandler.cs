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
        var recipientIds = await _identityService.GetUserIdsInRoleAsync(Roles.Administrator);

        foreach (var id in recipientIds)
        {
            var n = Notification.Create(id, "LeaveRequestSubmitted",
                "New Leave Request",
                $"A leave request was submitted for {notification.Request.StartDate} to {notification.Request.EndDate}.");
            _context.Notifications.Add(n);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
