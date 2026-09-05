namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleOilChangeRecord;

public class UpdateVehicleOilChangeRecordCommandValidator : AbstractValidator<UpdateVehicleOilChangeRecordCommand>
{
    public UpdateVehicleOilChangeRecordCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Odometer).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}
