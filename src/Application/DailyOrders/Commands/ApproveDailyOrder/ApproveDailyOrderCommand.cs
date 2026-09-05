using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.DailyOrders.Commands.ApproveDailyOrder;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record ApproveDailyOrderCommand : IRequest { public Guid DailyOrderId { get; init; } }
