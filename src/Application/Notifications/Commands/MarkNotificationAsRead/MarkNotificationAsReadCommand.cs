using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Notifications.Commands.MarkNotificationAsRead;

[Authorize]
public record MarkNotificationAsReadCommand : IRequest
{
    public Guid Id { get; init; }
}
