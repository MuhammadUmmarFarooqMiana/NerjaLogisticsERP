using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Employees.Queries;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PaginatedList<EmployeeListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetEmployeesQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<PaginatedList<EmployeeListItemDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Employees.AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(e => e.AccountStatus == request.Status);

        if (request.PlatformId.HasValue)
            query = query.Where(e => e.PlatformId == request.PlatformId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(e => e.FullName.Contains(term) || (e.IqamaNumber != null && e.IqamaNumber.Contains(term)));
        }

        // UserId is only needed here to resolve Roles below — it's dropped again once
        // the final EmployeeListItemDto is built and never reaches the wire.
        var page = await query
            .OrderBy(e => e.FullName)
            .Select(e => new
            {
                e.Id,
                e.UserId,
                e.FullName,
                AccountStatus = e.AccountStatus.ToString(),
                e.IqamaNumber,
                PlatformName = e.Platform != null ? e.Platform.Name : null,
                e.JoiningDate
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        // Four batched role-membership lookups instead of one per employee (N+1) —
        // mirrors the GetUserIdsInRoleAsync pattern already used for notifications.
        var riderIds = (await _identityService.GetUserIdsInRoleAsync(Roles.Rider)).ToHashSet();
        var supervisorIds = (await _identityService.GetUserIdsInRoleAsync(Roles.Supervisor)).ToHashSet();
        var accountantIds = (await _identityService.GetUserIdsInRoleAsync(Roles.Accountant)).ToHashSet();
        var administratorIds = (await _identityService.GetUserIdsInRoleAsync(Roles.Administrator)).ToHashSet();

        List<string> RolesFor(Guid userId)
        {
            var roles = new List<string>();
            if (administratorIds.Contains(userId)) roles.Add(Roles.Administrator);
            if (supervisorIds.Contains(userId)) roles.Add(Roles.Supervisor);
            if (accountantIds.Contains(userId)) roles.Add(Roles.Accountant);
            if (riderIds.Contains(userId)) roles.Add(Roles.Rider);
            return roles;
        }

        var items = page.Items
            .Select(e => new EmployeeListItemDto
            {
                Id = e.Id,
                FullName = e.FullName,
                AccountStatus = e.AccountStatus,
                IqamaNumber = e.IqamaNumber,
                PlatformName = e.PlatformName,
                JoiningDate = e.JoiningDate,
                Roles = RolesFor(e.UserId)
            })
            .ToList();

        var effectivePageSize = request.PageSize is > 0 ? request.PageSize.Value : Math.Max(items.Count, 1);
        return new PaginatedList<EmployeeListItemDto>(items, page.TotalCount, page.PageNumber, effectivePageSize);
    }
}
