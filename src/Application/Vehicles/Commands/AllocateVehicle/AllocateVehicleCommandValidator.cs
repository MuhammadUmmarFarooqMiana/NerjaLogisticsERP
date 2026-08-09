namespace NerjaLogisticsERP.Application.Vehicles.Commands.AllocateVehicle;

public class AllocateVehicleCommandValidator : AbstractValidator<AllocateVehicleCommand>
{
    public AllocateVehicleCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.AssignedDate).NotEmpty();
    }
}
