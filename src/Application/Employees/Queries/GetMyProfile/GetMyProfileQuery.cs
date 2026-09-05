using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Employees.Queries;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetMyProfile;

// No Roles on [Authorize] — any authenticated user may fetch their own record.
// Scoped to _user.Id so a Rider can never see anyone else's profile this way.
[Authorize]
public record GetMyProfileQuery : IRequest<EmployeeDetailDto>;
