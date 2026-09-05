namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleAccidentRecord;

public class AddVehicleAccidentRecordCommandValidator : AbstractValidator<AddVehicleAccidentRecordCommand>
{
    public AddVehicleAccidentRecordCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.RepairCost).GreaterThanOrEqualTo(0);
    }
}
