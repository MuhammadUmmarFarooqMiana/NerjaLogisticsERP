using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.DailyOrders.Queries.GetPendingApprovalsCount;

// Administrator sees the fleet-wide count; a plain Supervisor sees only their
// own reports' — mirrors GetDailyOrdersQuery's scoping for the same reviewers.
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record GetPendingDailyOrderApprovalsCountQuery : IRequest<int>;
