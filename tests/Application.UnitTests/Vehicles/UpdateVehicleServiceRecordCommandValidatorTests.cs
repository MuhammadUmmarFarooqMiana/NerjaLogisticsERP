using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleServiceRecord;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class UpdateVehicleServiceRecordCommandValidatorTests
{
    private readonly UpdateVehicleServiceRecordCommandValidator _validator = new();

    private static UpdateVehicleServiceRecordCommand Valid() => new()
    {
        Id = Guid.NewGuid(),
        Odometer = 10000,
        Description = "Oil and filter change.",
        Cost = 150m
    };

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(Valid() with { Id = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateVehicleServiceRecordCommand.Id));

    [Test]
    public void ShouldHaveError_WhenOdometerIsNegative()
        => _validator.Validate(Valid() with { Odometer = -1 })
            .ShouldHaveErrorFor(nameof(UpdateVehicleServiceRecordCommand.Odometer));

    [Test]
    public void ShouldHaveError_WhenDescriptionIsEmpty()
        => _validator.Validate(Valid() with { Description = "" })
            .ShouldHaveErrorFor(nameof(UpdateVehicleServiceRecordCommand.Description));

    [Test]
    public void ShouldHaveError_WhenCostIsNegative()
        => _validator.Validate(Valid() with { Cost = -1m })
            .ShouldHaveErrorFor(nameof(UpdateVehicleServiceRecordCommand.Cost));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
