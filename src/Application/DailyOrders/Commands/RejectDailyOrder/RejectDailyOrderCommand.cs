using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.DailyOrders.Commands.RejectDailyOrder;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record RejectDailyOrderCommand : IRequest
{
    public Guid DailyOrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
