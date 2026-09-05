using NerjaLogisticsERP.Application.Advances.Queries.GetAdvances;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Advances.Queries.GetMyAdvances;

public class GetMyAdvancesQueryHandler : IRequestHandler<GetMyAdvancesQuery, PaginatedList<AdvanceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyAdvancesQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginatedList<AdvanceDto>> Handle(GetMyAdvancesQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var query = _context.Advances.Include(a => a.Employee).Where(a => a.Employee.UserId == userId);

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
