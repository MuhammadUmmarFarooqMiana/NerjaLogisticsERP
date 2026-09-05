using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleOilChangeRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record UpdateVehicleOilChangeRecordCommand : IRequest
{
    public Guid Id { get; init; }
    public DateOnly ChangeDate { get; init; }
    public int Odometer { get; init; }
    public decimal Cost { get; init; }
}
