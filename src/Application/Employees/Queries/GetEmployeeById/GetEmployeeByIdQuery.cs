using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Employees.Queries;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Queries.GetEmployeeById;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record GetEmployeeByIdQuery : IRequest<EmployeeDetailDto>
{
    public Guid Id { get; init; }
}
