using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Commands.TerminateEmployee;

[Authorize(Roles = Roles.Administrator)]
public record TerminateEmployeeCommand : IRequest { public Guid Id { get; init; } }
