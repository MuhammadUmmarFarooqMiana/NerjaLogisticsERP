using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.ReturnVehicle;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class ReturnVehicleCommandValidatorTests
{
    private readonly ReturnVehicleCommandValidator _validator = new();

    private static ReturnVehicleCommand Valid() => new()
    {
        AllocationId = Guid.NewGuid(),
        ReturnedDate = new DateOnly(2026, 1, 1)
    };

    [Test]
    public void ShouldHaveError_WhenAllocationIdIsEmpty()
        => _validator.Validate(Valid() with { AllocationId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(ReturnVehicleCommand.AllocationId));

    [Test]
    public void ShouldHaveError_WhenReturnedDateIsDefault()
        => _validator.Validate(Valid() with { ReturnedDate = default })
            .ShouldHaveErrorFor(nameof(ReturnVehicleCommand.ReturnedDate));

    [Test]
    public void ShouldNotHaveErrors_ForAValidReturn()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
