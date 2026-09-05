using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleServiceRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record UpdateVehicleServiceRecordCommand : IRequest
{
    public Guid Id { get; init; }
    public DateOnly ServiceDate { get; init; }
    public int Odometer { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Cost { get; init; }
}
