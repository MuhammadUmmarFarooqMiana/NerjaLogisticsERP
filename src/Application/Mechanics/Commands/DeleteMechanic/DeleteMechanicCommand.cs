using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Mechanics.Commands.DeleteMechanic;

[Authorize(Roles = Roles.Administrator)]
public record DeleteMechanicCommand : IRequest
{
    public Guid Id { get; init; }
}
