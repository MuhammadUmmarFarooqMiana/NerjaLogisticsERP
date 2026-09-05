using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserListItemDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<List<UserListItemDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _identityService.GetAllUsersAsync();

        var employeeNames = await _context.Employees
            .Select(e => new { e.UserId, e.FullName })
            .ToDictionaryAsync(e => e.UserId, e => e.FullName, cancellationToken);

        return users
            .Select(u => new UserListItemDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = employeeNames.GetValueOrDefault(u.Id),
                Roles = u.Roles.ToList()
            })
            .OrderBy(u => u.FullName ?? u.Email)
            .ToList();
    }
}
