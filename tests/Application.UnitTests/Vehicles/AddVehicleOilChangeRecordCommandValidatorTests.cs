using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleOilChangeRecord;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class AddVehicleOilChangeRecordCommandValidatorTests
{
    private readonly AddVehicleOilChangeRecordCommandValidator _validator = new();

    private static AddVehicleOilChangeRecordCommand Valid() => new()
    {
        VehicleId = Guid.NewGuid(),
        Odometer = 10000,
        Cost = 80m
    };

    [Test]
    public void ShouldHaveError_WhenVehicleIdIsEmpty()
        => _validator.Validate(Valid() with { VehicleId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(AddVehicleOilChangeRecordCommand.VehicleId));

    [Test]
    public void ShouldHaveError_WhenOdometerIsNegative()
        => _validator.Validate(Valid() with { Odometer = -1 })
            .ShouldHaveErrorFor(nameof(AddVehicleOilChangeRecordCommand.Odometer));

    [Test]
    public void ShouldHaveError_WhenCostIsNegative()
        => _validator.Validate(Valid() with { Cost = -1m })
            .ShouldHaveErrorFor(nameof(AddVehicleOilChangeRecordCommand.Cost));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRecord()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
