namespace NerjaLogisticsERP.Application.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery : IRequest<List<NotificationDto>>
{
    public bool? UnreadOnly { get; init; }
}
