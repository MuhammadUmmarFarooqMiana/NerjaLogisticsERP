using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleOilChangeRecord;

[Authorize(Roles = Roles.Administrator)]
public record DeleteVehicleOilChangeRecordCommand : IRequest
{
    public Guid Id { get; init; }
}
