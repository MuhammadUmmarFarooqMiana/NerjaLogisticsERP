using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleOilChangeRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record AddVehicleOilChangeRecordCommand : IRequest<Guid>
{
    public Guid VehicleId { get; init; }
    public DateOnly ChangeDate { get; init; }
    public int Odometer { get; init; }
    public decimal Cost { get; init; }
}
