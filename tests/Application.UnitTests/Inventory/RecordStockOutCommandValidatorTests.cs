using NerjaLogisticsERP.Application.Inventory.Commands.RecordStockOut;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Inventory;

public class RecordStockOutCommandValidatorTests
{
    private readonly RecordStockOutCommandValidator _validator = new();

    private static RecordStockOutCommand Valid() => new()
    {
        ItemId = Guid.NewGuid(),
        EmployeeId = Guid.NewGuid(),
        MechanicId = Guid.NewGuid(),
        Quantity = 2
    };

    [Test]
    public void ShouldHaveError_WhenItemIdIsEmpty()
        => _validator.Validate(Valid() with { ItemId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(RecordStockOutCommand.ItemId));

    [Test]
    public void ShouldHaveError_WhenEmployeeIdIsEmpty()
        => _validator.Validate(Valid() with { EmployeeId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(RecordStockOutCommand.EmployeeId));

    [Test]
    public void ShouldHaveError_WhenMechanicIdIsEmpty()
        => _validator.Validate(Valid() with { MechanicId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(RecordStockOutCommand.MechanicId));

    [TestCase(0)]
    [TestCase(-1)]
    public void ShouldHaveError_WhenQuantityIsNotPositive(int quantity)
        => _validator.Validate(Valid() with { Quantity = quantity })
            .ShouldHaveErrorFor(nameof(RecordStockOutCommand.Quantity));

    [Test]
    public void ShouldNotHaveErrors_ForAValidStockOut()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
