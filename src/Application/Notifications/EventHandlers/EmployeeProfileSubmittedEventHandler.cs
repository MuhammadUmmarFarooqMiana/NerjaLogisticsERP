using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Events;

namespace NerjaLogisticsERP.Application.Notifications.EventHandlers;

public class EmployeeProfileSubmittedEventHandler : INotificationHandler<EmployeeProfileSubmittedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public EmployeeProfileSubmittedEventHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task Handle(EmployeeProfileSubmittedEvent notification, CancellationToken cancellationToken)
    {
        var adminIds = await _identityService.GetUserIdsInRoleAsync(Roles.Administrator);

        foreach (var adminId in adminIds)
        {
            var n = Notification.Create(adminId, "ProfilePendingReview",
                "New Profile Pending Review",
                $"{notification.Employee.FullName} submitted their profile for review.");
            _context.Notifications.Add(n);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
