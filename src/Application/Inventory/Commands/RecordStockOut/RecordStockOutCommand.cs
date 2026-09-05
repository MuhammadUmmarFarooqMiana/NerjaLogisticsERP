using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Inventory.Commands.RecordStockOut;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record RecordStockOutCommand : IRequest<Guid>
{
    public Guid ItemId { get; init; }
    public Guid EmployeeId { get; init; }
    // The mechanic who takes the parts to actually service the employee's
    // vehicle — required for every new stock-out (see StockOut.Create).
    public Guid MechanicId { get; init; }
    public int Quantity { get; init; }
    public DateOnly StockDate { get; init; }
}
