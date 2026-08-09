namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleServiceRecord;

public class AddVehicleServiceRecordCommandValidator : AbstractValidator<AddVehicleServiceRecordCommand>
{
    public AddVehicleServiceRecordCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.Odometer).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}
