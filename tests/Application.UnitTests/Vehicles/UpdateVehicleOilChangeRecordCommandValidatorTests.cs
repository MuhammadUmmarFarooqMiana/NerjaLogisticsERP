using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleOilChangeRecord;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class UpdateVehicleOilChangeRecordCommandValidatorTests
{
    private readonly UpdateVehicleOilChangeRecordCommandValidator _validator = new();

    private static UpdateVehicleOilChangeRecordCommand Valid() => new()
    {
        Id = Guid.NewGuid(),
        Odometer = 10000,
        Cost = 80m
    };

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(Valid() with { Id = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateVehicleOilChangeRecordCommand.Id));

    [Test]
    public void ShouldHaveError_WhenOdometerIsNegative()
        => _validator.Validate(Valid() with { Odometer = -1 })
            .ShouldHaveErrorFor(nameof(UpdateVehicleOilChangeRecordCommand.Odometer));

    [Test]
    public void ShouldHaveError_WhenCostIsNegative()
        => _validator.Validate(Valid() with { Cost = -1m })
            .ShouldHaveErrorFor(nameof(UpdateVehicleOilChangeRecordCommand.Cost));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
