using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.DailyOrders.Commands.CorrectDailyOrder;

// Only valid against a Rejected order — the Supervisor/Administrator is
// correcting the count they flagged as wrong, which finalizes it as Approved.
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record CorrectDailyOrderCommand : IRequest
{
    public Guid DailyOrderId { get; init; }
    public int CompletedOrders { get; init; }
    public string Reason { get; init; } = string.Empty;
}
