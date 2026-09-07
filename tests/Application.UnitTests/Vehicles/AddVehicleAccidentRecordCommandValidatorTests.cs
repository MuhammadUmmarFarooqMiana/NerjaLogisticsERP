using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleAccidentRecord;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class AddVehicleAccidentRecordCommandValidatorTests
{
    private readonly AddVehicleAccidentRecordCommandValidator _validator = new();

    private static AddVehicleAccidentRecordCommand Valid() => new()
    {
        VehicleId = Guid.NewGuid(),
        Description = "Rear bumper damage.",
        RepairCost = 500m
    };

    [Test]
    public void ShouldHaveError_WhenVehicleIdIsEmpty()
        => _validator.Validate(Valid() with { VehicleId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(AddVehicleAccidentRecordCommand.VehicleId));

    [Test]
    public void ShouldHaveError_WhenDescriptionIsEmpty()
        => _validator.Validate(Valid() with { Description = "" })
            .ShouldHaveErrorFor(nameof(AddVehicleAccidentRecordCommand.Description));

    [Test]
    public void ShouldHaveError_WhenRepairCostIsNegative()
        => _validator.Validate(Valid() with { RepairCost = -1m })
            .ShouldHaveErrorFor(nameof(AddVehicleAccidentRecordCommand.RepairCost));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRecord()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
