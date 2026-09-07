using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleTyreReplacementRecord;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class AddVehicleTyreReplacementRecordCommandValidatorTests
{
    private readonly AddVehicleTyreReplacementRecordCommandValidator _validator = new();

    private static AddVehicleTyreReplacementRecordCommand Valid() => new()
    {
        VehicleId = Guid.NewGuid(),
        Odometer = 10000,
        NumberOfTyres = 4,
        Cost = 800m
    };

    [Test]
    public void ShouldHaveError_WhenVehicleIdIsEmpty()
        => _validator.Validate(Valid() with { VehicleId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(AddVehicleTyreReplacementRecordCommand.VehicleId));

    [Test]
    public void ShouldHaveError_WhenOdometerIsNegative()
        => _validator.Validate(Valid() with { Odometer = -1 })
            .ShouldHaveErrorFor(nameof(AddVehicleTyreReplacementRecordCommand.Odometer));

    [TestCase(0)]
    [TestCase(7)]
    public void ShouldHaveError_WhenNumberOfTyresIsOutOfRange(int numberOfTyres)
        => _validator.Validate(Valid() with { NumberOfTyres = numberOfTyres })
            .ShouldHaveErrorFor(nameof(AddVehicleTyreReplacementRecordCommand.NumberOfTyres));

    [Test]
    public void ShouldHaveError_WhenCostIsNegative()
        => _validator.Validate(Valid() with { Cost = -1m })
            .ShouldHaveErrorFor(nameof(AddVehicleTyreReplacementRecordCommand.Cost));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRecord()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
