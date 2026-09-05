using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleAccidentRecord;

[Authorize(Roles = Roles.Administrator)]
public record DeleteVehicleAccidentRecordCommand : IRequest
{
    public Guid Id { get; init; }
}
