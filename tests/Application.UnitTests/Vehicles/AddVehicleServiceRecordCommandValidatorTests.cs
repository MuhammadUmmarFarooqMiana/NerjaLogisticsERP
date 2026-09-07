using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleServiceRecord;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class AddVehicleServiceRecordCommandValidatorTests
{
    private readonly AddVehicleServiceRecordCommandValidator _validator = new();

    private static AddVehicleServiceRecordCommand Valid() => new()
    {
        VehicleId = Guid.NewGuid(),
        Odometer = 10000,
        Description = "Oil and filter change.",
        Cost = 150m
    };

    [Test]
    public void ShouldHaveError_WhenVehicleIdIsEmpty()
        => _validator.Validate(Valid() with { VehicleId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(AddVehicleServiceRecordCommand.VehicleId));

    [Test]
    public void ShouldHaveError_WhenOdometerIsNegative()
        => _validator.Validate(Valid() with { Odometer = -1 })
            .ShouldHaveErrorFor(nameof(AddVehicleServiceRecordCommand.Odometer));

    [Test]
    public void ShouldHaveError_WhenDescriptionIsEmpty()
        => _validator.Validate(Valid() with { Description = "" })
            .ShouldHaveErrorFor(nameof(AddVehicleServiceRecordCommand.Description));

    [Test]
    public void ShouldHaveError_WhenCostIsNegative()
        => _validator.Validate(Valid() with { Cost = -1m })
            .ShouldHaveErrorFor(nameof(AddVehicleServiceRecordCommand.Cost));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRecord()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
