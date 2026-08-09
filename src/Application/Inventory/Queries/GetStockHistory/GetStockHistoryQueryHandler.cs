using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Application.Inventory.Queries.GetStockHistory;

public class GetStockHistoryQueryHandler : IRequestHandler<GetStockHistoryQuery, List<StockMovementDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockHistoryQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<StockMovementDto>> Handle(GetStockHistoryQuery request, CancellationToken cancellationToken)
    {
        var ins = await _context.StockIns
            .Where(s => s.ItemId == request.ItemId)
            .Select(s => new StockMovementDto(s.Id, "In", s.Quantity, s.StockDate, s.Supplier.Name))
            .ToListAsync(cancellationToken);

        var outs = await _context.StockOuts
            .Where(s => s.ItemId == request.ItemId)
            .Select(s => new StockMovementDto(s.Id, "Out", s.Quantity, s.StockDate, s.Employee.FullName))
            .ToListAsync(cancellationToken);

        return ins.Concat(outs).OrderByDescending(m => m.Date).ToList();
    }
}
