using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Advances.Queries.GetAdvances;

public class GetAdvancesQueryHandler : IRequestHandler<GetAdvancesQuery, PaginatedList<AdvanceDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAdvancesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<AdvanceDto>> Handle(GetAdvancesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Advances.Include(a => a.Employee).AsQueryable();

        if (request.EmployeeId.HasValue)
            query = query.Where(a => a.EmployeeId == request.EmployeeId);

        if (request.StartDate.HasValue)
            query = query.Where(a => a.AdvanceDate >= request.StartDate);

        if (request.EndDate.HasValue)
            query = query.Where(a => a.AdvanceDate <= request.EndDate);

        return await query
            .OrderByDescending(a => a.AdvanceDate)
            .Select(a => new AdvanceDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                Amount = a.Amount,
                AdvanceDate = a.AdvanceDate,
                Remarks = a.Remarks
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
