using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleAccidentRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record AddVehicleAccidentRecordCommand : IRequest<Guid>
{
    public Guid VehicleId { get; init; }
    public DateOnly AccidentDate { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal RepairCost { get; init; }
}
