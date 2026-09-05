using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.LeaveRequest.Queries;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.LeaveRequest.Queries.GetLeaveRequests;

public class GetLeaveRequestsQueryHandler : IRequestHandler<GetLeaveRequestsQuery, PaginatedList<LeaveRequestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetLeaveRequestsQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginatedList<LeaveRequestDto>> Handle(GetLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var isSupervisor = _user.Roles?.Contains(Roles.Supervisor) ?? false;
        var isAdministrator = _user.Roles?.Contains(Roles.Administrator) ?? false;

        var query = _context.LeaveRequests.AsQueryable();

        if (isSupervisor && !isAdministrator)
        {
            var userId = _user.Id!.Value;
            var supervisorEmployeeId = await _context.Employees
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);

            query = query.Where(l => l.Employee.SupervisorId == supervisorEmployeeId);
        }

        if (request.Status.HasValue)
            query = query.Where(l => l.Status == request.Status);

        if (request.EmployeeId.HasValue)
            query = query.Where(l => l.EmployeeId == request.EmployeeId);

        return await query
            .OrderByDescending(l => l.Created)
            .Select(l => new LeaveRequestDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.FullName,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status.ToString(),
                ReviewedByName = l.ReviewedBy != null
                    ? _context.Employees.Where(e => e.UserId == l.ReviewedBy).Select(e => e.FullName).FirstOrDefault()
                    : null,
                ReviewedAt = l.ReviewedAt,
                RejectionReason = l.RejectionReason
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
