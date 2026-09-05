namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleServiceRecord;

public class UpdateVehicleServiceRecordCommandValidator : AbstractValidator<UpdateVehicleServiceRecordCommand>
{
    public UpdateVehicleServiceRecordCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Odometer).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}
