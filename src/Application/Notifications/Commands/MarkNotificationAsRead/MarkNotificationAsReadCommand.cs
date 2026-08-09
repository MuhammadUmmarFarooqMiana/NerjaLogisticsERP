namespace NerjaLogisticsERP.Application.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand : IRequest
{
    public Guid Id { get; init; }
}
