using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAccidentRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record UpdateVehicleAccidentRecordCommand : IRequest
{
    public Guid Id { get; init; }
    public DateOnly AccidentDate { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal RepairCost { get; init; }
}
