using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Employees.Queries;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetSupervisors;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record GetSupervisorsQuery : IRequest<List<SupervisorLookupDto>>;
