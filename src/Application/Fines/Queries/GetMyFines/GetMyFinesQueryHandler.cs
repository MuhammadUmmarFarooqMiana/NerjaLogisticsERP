using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Fines.Queries.GetFines;

namespace NerjaLogisticsERP.Application.Fines.Queries.GetMyFines;

public class GetMyFinesQueryHandler : IRequestHandler<GetMyFinesQuery, PaginatedList<FineDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyFinesQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginatedList<FineDto>> Handle(GetMyFinesQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var query = _context.Fines.Include(f => f.Employee).Where(f => f.Employee.UserId == userId);

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
