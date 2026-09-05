namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAccidentRecord;

public class UpdateVehicleAccidentRecordCommandValidator : AbstractValidator<UpdateVehicleAccidentRecordCommand>
{
    public UpdateVehicleAccidentRecordCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.RepairCost).GreaterThanOrEqualTo(0);
    }
}
