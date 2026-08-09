namespace NerjaLogisticsERP.Application.Vehicles.Commands.ReturnVehicle;

public class ReturnVehicleCommandValidator : AbstractValidator<ReturnVehicleCommand>
{
    public ReturnVehicleCommandValidator()
    {
        RuleFor(x => x.AllocationId).NotEmpty();
        RuleFor(x => x.ReturnedDate).NotEmpty();
    }
}
