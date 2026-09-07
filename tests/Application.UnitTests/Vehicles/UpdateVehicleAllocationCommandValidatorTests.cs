using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAllocation;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class UpdateVehicleAllocationCommandValidatorTests
{
    private readonly UpdateVehicleAllocationCommandValidator _validator = new();

    private static UpdateVehicleAllocationCommand Valid() => new()
    {
        Id = Guid.NewGuid(),
        EmployeeId = Guid.NewGuid(),
        AssignedDate = new DateOnly(2026, 1, 1)
    };

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(Valid() with { Id = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateVehicleAllocationCommand.Id));

    [Test]
    public void ShouldHaveError_WhenEmployeeIdIsEmpty()
        => _validator.Validate(Valid() with { EmployeeId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateVehicleAllocationCommand.EmployeeId));

    [Test]
    public void ShouldHaveError_WhenAssignedDateIsDefault()
        => _validator.Validate(Valid() with { AssignedDate = default })
            .ShouldHaveErrorFor(nameof(UpdateVehicleAllocationCommand.AssignedDate));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
