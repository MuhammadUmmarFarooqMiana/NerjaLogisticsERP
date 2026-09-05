using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleServiceRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record AddVehicleServiceRecordCommand : IRequest<Guid>
{
    public Guid VehicleId { get; init; }
    public DateOnly ServiceDate { get; init; }
    public int Odometer { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Cost { get; init; }
}
