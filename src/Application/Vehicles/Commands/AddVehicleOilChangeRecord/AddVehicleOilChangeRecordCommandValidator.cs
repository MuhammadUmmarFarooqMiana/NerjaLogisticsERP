namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleOilChangeRecord;

public class AddVehicleOilChangeRecordCommandValidator : AbstractValidator<AddVehicleOilChangeRecordCommand>
{
    public AddVehicleOilChangeRecordCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.Odometer).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}
