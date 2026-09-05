namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleTyreReplacementRecord;

public class AddVehicleTyreReplacementRecordCommandValidator : AbstractValidator<AddVehicleTyreReplacementRecordCommand>
{
    public AddVehicleTyreReplacementRecordCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.Odometer).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NumberOfTyres).GreaterThan(0).LessThanOrEqualTo(6);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}
