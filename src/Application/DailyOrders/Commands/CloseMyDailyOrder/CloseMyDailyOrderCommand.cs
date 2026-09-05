using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.DailyOrders.Commands.CloseMyDailyOrder;

// Closes the caller's own daily order for today — no id, self-scoped only.
[Authorize]
public record CloseMyDailyOrderCommand : IRequest;
