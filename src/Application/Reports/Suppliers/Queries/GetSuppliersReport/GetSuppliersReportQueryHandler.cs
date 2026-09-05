using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Suppliers.Queries.GetSuppliersReport;

public class GetSuppliersReportQueryHandler : IRequestHandler<GetSuppliersReportQuery, SuppliersReportDto>
{
    private readonly IApplicationDbContext _context;
    public GetSuppliersReportQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<SuppliersReportDto> Handle(GetSuppliersReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        var query = _context.StockIns.Include(s => s.Supplier).Include(s => s.Item)
            .Where(s => s.StockDate >= period.Start && s.StockDate <= period.End);

        if (request.SupplierId.HasValue)
            query = query.Where(s => s.SupplierId == request.SupplierId);

        var rows = await query
            .OrderBy(s => s.StockDate).ThenBy(s => s.Supplier.Name)
            .Select(s => new SuppliersReportRowDto
            {
                SupplierName = s.Supplier.Name,
                ItemName = s.Item.ItemName,
                Quantity = s.Quantity,
                StockDate = s.StockDate
            })
            .ToListAsync(cancellationToken);

        return new SuppliersReportDto
        {
            PeriodType = request.PeriodType.ToString(),
            PeriodLabel = period.Label,
            PeriodStart = period.Start,
            PeriodEnd = period.End,
            TotalSuppliers = rows.Select(r => r.SupplierName).Distinct().Count(),
            TotalTransactions = rows.Count,
            TotalQuantityReceived = rows.Sum(r => r.Quantity),
            Rows = rows
        };
    }
}
