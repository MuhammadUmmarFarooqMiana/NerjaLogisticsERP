using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Notifications.Queries.GetMyNotifications;

[Authorize]
public record GetMyNotificationsQuery : IRequest<List<NotificationDto>>
{
    public bool? UnreadOnly { get; init; }
}
