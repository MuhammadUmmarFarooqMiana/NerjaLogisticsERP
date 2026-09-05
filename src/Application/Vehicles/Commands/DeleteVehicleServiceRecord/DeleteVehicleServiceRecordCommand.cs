using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleServiceRecord;

[Authorize(Roles = Roles.Administrator)]
public record DeleteVehicleServiceRecordCommand : IRequest
{
    public Guid Id { get; init; }
}
