using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanics;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanicById;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetMechanicByIdQuery : IRequest<MechanicDto>
{
    public Guid Id { get; init; }
}
