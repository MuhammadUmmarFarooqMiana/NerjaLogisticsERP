using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleTyreReplacementRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record UpdateVehicleTyreReplacementRecordCommand : IRequest
{
    public Guid Id { get; init; }
    public DateOnly ReplacementDate { get; init; }
    public int Odometer { get; init; }
    public int NumberOfTyres { get; init; }
    public decimal Cost { get; init; }
}
