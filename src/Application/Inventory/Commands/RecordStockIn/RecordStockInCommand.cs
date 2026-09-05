using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Inventory.Commands.RecordStockIn;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record RecordStockInCommand : IRequest<Guid>
{
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public DateOnly StockDate { get; init; }
    public Guid SupplierId { get; init; }
}
