using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Employees.Queries;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetSupervisors;

public class GetSupervisorsQueryHandler : IRequestHandler<GetSupervisorsQuery, List<SupervisorLookupDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetSupervisorsQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<List<SupervisorLookupDto>> Handle(GetSupervisorsQuery request, CancellationToken cancellationToken)
    {
        var supervisorUserIds = await _identityService.GetUserIdsInRoleAsync(Roles.Supervisor);

        return await _context.Employees
            .Where(e => supervisorUserIds.Contains(e.UserId) && e.AccountStatus == AccountStatus.Active)
            .OrderBy(e => e.FullName)
            .Select(e => new SupervisorLookupDto { Id = e.Id, FullName = e.FullName })
            .ToListAsync(cancellationToken);
    }
}
