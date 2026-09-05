using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Commands.DeleteEmployee;

[Authorize(Roles = Roles.Administrator)]
public record DeleteEmployeeCommand : IRequest { public Guid Id { get; init; } }
