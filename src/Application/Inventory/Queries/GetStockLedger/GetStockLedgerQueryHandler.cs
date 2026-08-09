using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Application.Inventory.Queries.GetStockLedger;

public class GetStockLedgerQueryHandler : IRequestHandler<GetStockLedgerQuery, List<StockLedgerEntryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockLedgerQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<StockLedgerEntryDto>> Handle(GetStockLedgerQuery request, CancellationToken cancellationToken)
    {
        var insQuery = _context.StockIns.AsQueryable();
        var outsQuery = _context.StockOuts.AsQueryable();

        if (request.ItemId.HasValue)
        {
            insQuery = insQuery.Where(s => s.ItemId == request.ItemId);
            outsQuery = outsQuery.Where(s => s.ItemId == request.ItemId);
        }

        if (request.FromDate.HasValue)
        {
            insQuery = insQuery.Where(s => s.StockDate >= request.FromDate);
            outsQuery = outsQuery.Where(s => s.StockDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            insQuery = insQuery.Where(s => s.StockDate <= request.ToDate);
            outsQuery = outsQuery.Where(s => s.StockDate <= request.ToDate);
        }

        var ins = await insQuery
            .Select(s => new StockLedgerEntryDto
            {
                Id = s.Id,
                ItemId = s.ItemId,
                ItemName = s.Item.ItemName,
                Type = "In",
                Quantity = s.Quantity,
                Date = s.StockDate,
                Detail = s.Supplier.Name
            })
            .ToListAsync(cancellationToken);

        var outs = await outsQuery
            .Select(s => new StockLedgerEntryDto
            {
                Id = s.Id,
                ItemId = s.ItemId,
                ItemName = s.Item.ItemName,
                Type = "Out",
                Quantity = s.Quantity,
                Date = s.StockDate,
                Detail = s.Employee.FullName
            })
            .ToListAsync(cancellationToken);

        return ins.Concat(outs)
            .OrderByDescending(e => e.Date)
            .ToList();
    }
}
