using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Commands.ReactivateEmployee;

[Authorize(Roles = Roles.Administrator)]
public record ReactivateEmployeeCommand : IRequest { public Guid Id { get; init; } }
