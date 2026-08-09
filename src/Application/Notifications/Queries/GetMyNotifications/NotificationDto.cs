namespace NerjaLogisticsERP.Application.Notifications.Queries.GetMyNotifications;

public record NotificationDto(Guid Id, string Type, string Title, string Message, bool IsRead, DateTimeOffset Created);
