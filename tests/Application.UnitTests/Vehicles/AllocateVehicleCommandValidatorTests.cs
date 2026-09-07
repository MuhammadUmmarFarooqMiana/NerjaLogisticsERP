using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.AllocateVehicle;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class AllocateVehicleCommandValidatorTests
{
    private readonly AllocateVehicleCommandValidator _validator = new();

    private static AllocateVehicleCommand Valid() => new()
    {
        VehicleId = Guid.NewGuid(),
        EmployeeId = Guid.NewGuid(),
        AssignedDate = new DateOnly(2026, 1, 1)
    };

    [Test]
    public void ShouldHaveError_WhenVehicleIdIsEmpty()
        => _validator.Validate(Valid() with { VehicleId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(AllocateVehicleCommand.VehicleId));

    [Test]
    public void ShouldHaveError_WhenEmployeeIdIsEmpty()
        => _validator.Validate(Valid() with { EmployeeId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(AllocateVehicleCommand.EmployeeId));

    [Test]
    public void ShouldHaveError_WhenAssignedDateIsDefault()
        => _validator.Validate(Valid() with { AssignedDate = default })
            .ShouldHaveErrorFor(nameof(AllocateVehicleCommand.AssignedDate));

    [Test]
    public void ShouldNotHaveErrors_ForAValidAllocation()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
