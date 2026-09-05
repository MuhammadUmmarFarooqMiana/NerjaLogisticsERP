using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Inventory.Commands.RecordStockIn;

public class RecordStockInCommandHandler : IRequestHandler<RecordStockInCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly ILogger<RecordStockInCommandHandler> _logger;

    public RecordStockInCommandHandler(IApplicationDbContext context, IUser currentUser, ILogger<RecordStockInCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Guid> Handle(RecordStockInCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems.FindAsync(new object[] { request.ItemId }, cancellationToken)
            ?? throw new NotFoundException(nameof(InventoryItem), request.ItemId.ToString());

        var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == request.SupplierId, cancellationToken);
        if (!supplierExists)
            throw new NotFoundException(nameof(Supplier), request.SupplierId.ToString());

        var stockIn = StockIn.Create(request.ItemId, request.Quantity, request.StockDate, request.SupplierId, _currentUser.Id!.Value);

        item.ReceiveStock(request.Quantity);

        _context.StockIns.Add(stockIn);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Stock in: {Quantity} of {Item} from Supplier {SupplierId}, new balance {Balance}",
            request.Quantity, item.ItemName, request.SupplierId, item.CurrentStock);
        return stockIn.Id;
    }
}
