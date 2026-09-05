namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleTyreReplacementRecord;

public class UpdateVehicleTyreReplacementRecordCommandValidator : AbstractValidator<UpdateVehicleTyreReplacementRecordCommand>
{
    public UpdateVehicleTyreReplacementRecordCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Odometer).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NumberOfTyres).GreaterThan(0).LessThanOrEqualTo(6);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}
