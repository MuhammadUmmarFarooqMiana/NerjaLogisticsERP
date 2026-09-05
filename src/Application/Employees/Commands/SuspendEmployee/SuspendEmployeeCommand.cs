using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Commands.SuspendEmployee;

[Authorize(Roles = Roles.Administrator)]
public record SuspendEmployeeCommand : IRequest { public Guid Id { get; init; } }
