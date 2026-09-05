namespace NerjaLogisticsERP.Application.DailyOrders.Queries;

public record DailyOrderDto
{
    public Guid Id { get; init; }
    public DateOnly OrderDate { get; init; }
    public int CompletedOrders { get; init; }
    public string Status { get; init; } = default!;
    public DateTimeOffset? ClosedAt { get; init; }
    public string? ReviewNote { get; init; }
}
