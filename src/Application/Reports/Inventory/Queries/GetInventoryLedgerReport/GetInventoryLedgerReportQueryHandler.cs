using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Inventory.Queries.GetInventoryLedgerReport;

public class GetInventoryLedgerReportQueryHandler : IRequestHandler<GetInventoryLedgerReportQuery, InventoryLedgerReportDto>
{
    private readonly IApplicationDbContext _context;
    public GetInventoryLedgerReportQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<InventoryLedgerReportDto> Handle(GetInventoryLedgerReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        var itemsQuery = _context.InventoryItems.AsQueryable();
        if (request.ItemId.HasValue)
            itemsQuery = itemsQuery.Where(i => i.Id == request.ItemId);
        var itemNames = await itemsQuery.ToDictionaryAsync(i => i.Id, i => i.ItemName, cancellationToken);
        var itemIds = itemNames.Keys.ToList();

        // Opening balance per item = every movement strictly before the period, so the
        // ledger's running balance starts from a real number rather than zero.
        var openingIn = await _context.StockIns
            .Where(s => itemIds.Contains(s.ItemId) && s.StockDate < period.Start)
            .GroupBy(s => s.ItemId)
            .Select(g => new { ItemId = g.Key, Qty = g.Sum(s => s.Quantity) })
            .ToDictionaryAsync(x => x.ItemId, x => x.Qty, cancellationToken);
        var openingOut = await _context.StockOuts
            .Where(s => itemIds.Contains(s.ItemId) && s.StockDate < period.Start)
            .GroupBy(s => s.ItemId)
            .Select(g => new { ItemId = g.Key, Qty = g.Sum(s => s.Quantity) })
            .ToDictionaryAsync(x => x.ItemId, x => x.Qty, cancellationToken);

        var stockIns = await _context.StockIns.Include(s => s.Supplier)
            .Where(s => itemIds.Contains(s.ItemId) && s.StockDate >= period.Start && s.StockDate <= period.End)
            .ToListAsync(cancellationToken);
        var stockOuts = await _context.StockOuts.Include(s => s.Employee).Include(s => s.Mechanic)
            .Where(s => itemIds.Contains(s.ItemId) && s.StockDate >= period.Start && s.StockDate <= period.End)
            .ToListAsync(cancellationToken);

        var inMovements = stockIns.Select(s => new { s.ItemId, s.StockDate, Type = "In", Signed = s.Quantity, Reference = s.Supplier.Name, s.Created });
        var outMovements = stockOuts.Select(s => new
        {
            s.ItemId,
            s.StockDate,
            Type = "Out",
            Signed = -s.Quantity,
            Reference = s.Mechanic != null ? $"{s.Employee.FullName} (Mechanic: {s.Mechanic.Name})" : s.Employee.FullName,
            s.Created
        });

        // Grouped by item, chronological within each item — a running balance only makes
        // sense as a per-item sequence, not a single fleet-wide timeline.
        var movements = inMovements.Concat(outMovements)
            .OrderBy(m => m.ItemId).ThenBy(m => m.StockDate).ThenBy(m => m.Created)
            .ToList();

        var runningBalance = itemIds.ToDictionary(id => id, id => openingIn.GetValueOrDefault(id) - openingOut.GetValueOrDefault(id));

        var rows = new List<InventoryLedgerReportRowDto>();
        foreach (var movement in movements)
        {
            runningBalance[movement.ItemId] += movement.Signed;
            rows.Add(new InventoryLedgerReportRowDto
            {
                Date = movement.StockDate,
                ItemName = itemNames[movement.ItemId],
                MovementType = movement.Type,
                Quantity = Math.Abs(movement.Signed),
                Reference = movement.Reference,
                RunningBalance = runningBalance[movement.ItemId]
            });
        }

        return new InventoryLedgerReportDto
        {
            PeriodType = request.PeriodType.ToString(),
            PeriodLabel = period.Label,
            PeriodStart = period.Start,
            PeriodEnd = period.End,
            TotalIn = stockIns.Sum(s => s.Quantity),
            TotalOut = stockOuts.Sum(s => s.Quantity),
            NetChange = stockIns.Sum(s => s.Quantity) - stockOuts.Sum(s => s.Quantity),
            Rows = rows
        };
    }
}
