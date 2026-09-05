using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.DailyOrders.Queries;

namespace NerjaLogisticsERP.Application.DailyOrders.Queries.GetMyDailyOrder;

// No id — always the caller's own record for today. Null result means the
// rider hasn't logged any orders yet today, not an error.
[Authorize]
public record GetMyDailyOrderQuery : IRequest<DailyOrderDto?>;
