using System;
using System.Collections.Generic;
using System.Text;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.RegistrationNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.VehicleType).IsInEnum();
    }
}
