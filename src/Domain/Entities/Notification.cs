namespace NerjaLogisticsERP.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    private Notification() { }
    private Notification(Guid userId, string type, string title, string message)
    {
        UserId = userId;
        Type = type;
        Title = title;
        Message = message;
    }

    public Guid UserId { get; private set; }
    public string Type { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public bool IsRead { get; private set; }

    public static Notification Create(Guid userId, string type, string title, string message)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        return new Notification(userId, type, title, message);
    }

    public void MarkAsRead() => IsRead = true;
}
