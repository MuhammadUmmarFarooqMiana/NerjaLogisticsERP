using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAccidentRecord;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Vehicles;

public class UpdateVehicleAccidentRecordCommandValidatorTests
{
    private readonly UpdateVehicleAccidentRecordCommandValidator _validator = new();

    private static UpdateVehicleAccidentRecordCommand Valid() => new()
    {
        Id = Guid.NewGuid(),
        Description = "Rear bumper damage.",
        RepairCost = 500m
    };

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(Valid() with { Id = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateVehicleAccidentRecordCommand.Id));

    [Test]
    public void ShouldHaveError_WhenDescriptionIsEmpty()
        => _validator.Validate(Valid() with { Description = "" })
            .ShouldHaveErrorFor(nameof(UpdateVehicleAccidentRecordCommand.Description));

    [Test]
    public void ShouldHaveError_WhenRepairCostIsNegative()
        => _validator.Validate(Valid() with { RepairCost = -1m })
            .ShouldHaveErrorFor(nameof(UpdateVehicleAccidentRecordCommand.RepairCost));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
