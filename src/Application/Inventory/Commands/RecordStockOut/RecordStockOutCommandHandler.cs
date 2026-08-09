using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Inventory.Commands.RecordStockOut;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public class RecordStockOutCommandHandler : IRequestHandler<RecordStockOutCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<RecordStockOutCommandHandler> _logger;

    public RecordStockOutCommandHandler(IApplicationDbContext context, IUser currentUser, ILogger<RecordStockOutCommandHandler> logger)
    { _context = context; _currentUser = currentUser; _logger = logger; }

    public async Task<Guid> Handle(RecordStockOutCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems.FindAsync(new object[] { request.ItemId }, cancellationToken)
            ?? throw new NotFoundException(nameof(InventoryItem), request.ItemId.ToString());

        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (!employeeExists) throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        item.IssueStock(request.Quantity);   // throws InvalidOperationException if insufficient — mapped below

        var stockOut = StockOut.Create(request.ItemId, request.EmployeeId, request.Quantity, request.StockDate, _currentUser.Id!.Value);
        _context.StockOuts.Add(stockOut);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Stock out: {Quantity} of {Item} to Employee {EmployeeId}, new balance {Balance}",
            request.Quantity, item.ItemName, request.EmployeeId, item.CurrentStock);
        return stockOut.Id;
    }
}
