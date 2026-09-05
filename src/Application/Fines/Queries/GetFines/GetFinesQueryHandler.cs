using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Fines.Queries.GetFines;

public class GetFinesQueryHandler : IRequestHandler<GetFinesQuery, PaginatedList<FineDto>>
{
    private readonly IApplicationDbContext _context;
    public GetFinesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<FineDto>> Handle(GetFinesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Fines.Include(f => f.Employee).AsQueryable();

        if (request.EmployeeId.HasValue)
            query = query.Where(f => f.EmployeeId == request.EmployeeId);

        if (request.StartDate.HasValue)
            query = query.Where(f => f.FineDate >= request.StartDate);

        if (request.EndDate.HasValue)
            query = query.Where(f => f.FineDate <= request.EndDate);

        return await query
            .OrderByDescending(f => f.FineDate)
            .Select(f => new FineDto
            {
                Id = f.Id,
                EmployeeId = f.EmployeeId,
                EmployeeName = f.Employee.FullName,
                Amount = f.Amount,
                Reason = f.Reason,
                FineDate = f.FineDate
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
