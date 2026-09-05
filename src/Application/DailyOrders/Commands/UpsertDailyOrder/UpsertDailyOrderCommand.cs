using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;

// No EmployeeId here by design — it's resolved server-side from the caller's
// own identity so one rider can never overwrite another rider's count.
[Authorize]
public record UpsertDailyOrderCommand : IRequest<Guid>
{
    public int CompletedOrders { get; init; }
}
