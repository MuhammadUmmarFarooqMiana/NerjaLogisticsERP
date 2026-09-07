using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleTyreReplacementRecord;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class UpdateVehicleTyreReplacementRecordCommandValidatorTests
{
    private readonly UpdateVehicleTyreReplacementRecordCommandValidator _validator = new();

    private static UpdateVehicleTyreReplacementRecordCommand Valid() => new()
    {
        Id = Guid.NewGuid(),
        Odometer = 10000,
        NumberOfTyres = 4,
        Cost = 800m
    };

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(Valid() with { Id = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateVehicleTyreReplacementRecordCommand.Id));

    [Test]
    public void ShouldHaveError_WhenOdometerIsNegative()
        => _validator.Validate(Valid() with { Odometer = -1 })
            .ShouldHaveErrorFor(nameof(UpdateVehicleTyreReplacementRecordCommand.Odometer));

    [TestCase(0)]
    [TestCase(7)]
    public void ShouldHaveError_WhenNumberOfTyresIsOutOfRange(int numberOfTyres)
        => _validator.Validate(Valid() with { NumberOfTyres = numberOfTyres })
            .ShouldHaveErrorFor(nameof(UpdateVehicleTyreReplacementRecordCommand.NumberOfTyres));

    [Test]
    public void ShouldHaveError_WhenCostIsNegative()
        => _validator.Validate(Valid() with { Cost = -1m })
            .ShouldHaveErrorFor(nameof(UpdateVehicleTyreReplacementRecordCommand.Cost));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
