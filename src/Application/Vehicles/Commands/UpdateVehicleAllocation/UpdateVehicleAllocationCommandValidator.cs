namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAllocation;

public class UpdateVehicleAllocationCommandValidator : AbstractValidator<UpdateVehicleAllocationCommand>
{
    public UpdateVehicleAllocationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.AssignedDate).NotEmpty();
    }
}
