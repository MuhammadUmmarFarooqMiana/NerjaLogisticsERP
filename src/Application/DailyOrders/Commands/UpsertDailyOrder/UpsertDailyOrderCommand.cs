namespace NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;

public record UpsertDailyOrderCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public int CompletedOrders { get; init; }
}
