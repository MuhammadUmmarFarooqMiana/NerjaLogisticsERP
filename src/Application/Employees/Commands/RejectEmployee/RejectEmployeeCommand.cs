using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Commands.RejectEmployee;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record RejectEmployeeCommand : IRequest
{
    public Guid EmployeeId { get; init; }
    public string Reason { get; init; } = default!;
}
