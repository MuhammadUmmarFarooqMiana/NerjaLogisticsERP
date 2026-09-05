using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleTyreReplacementRecord;

[Authorize(Roles = Roles.Administrator)]
public record DeleteVehicleTyreReplacementRecordCommand : IRequest
{
    public Guid Id { get; init; }
}
