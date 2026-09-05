namespace NerjaLogisticsERP.Application.DailyOrders.Queries;

public record DailyOrderListItemDto
{
    /// <summary>Null when the row is a synthetic placeholder for a rider who hasn't logged anything yet today.</summary>
    public Guid? Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = default!;
    public DateOnly OrderDate { get; init; }
    public int CompletedOrders { get; init; }
    public string Status { get; init; } = default!;
    public DateTimeOffset? ClosedAt { get; init; }
    public string? ReviewNote { get; init; }
}
