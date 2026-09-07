using NerjaLogisticsERP.Application.Inventory.Commands.RecordStockIn;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Inventory;

public class RecordStockInCommandValidatorTests
{
    private readonly RecordStockInCommandValidator _validator = new();

    private static RecordStockInCommand Valid() => new()
    {
        ItemId = Guid.NewGuid(),
        SupplierId = Guid.NewGuid(),
        Quantity = 10
    };

    [Test]
    public void ShouldHaveError_WhenItemIdIsEmpty()
        => _validator.Validate(Valid() with { ItemId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(RecordStockInCommand.ItemId));

    [Test]
    public void ShouldHaveError_WhenSupplierIdIsEmpty()
        => _validator.Validate(Valid() with { SupplierId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(RecordStockInCommand.SupplierId));

    [TestCase(0)]
    [TestCase(-1)]
    public void ShouldHaveError_WhenQuantityIsNotPositive(int quantity)
        => _validator.Validate(Valid() with { Quantity = quantity })
            .ShouldHaveErrorFor(nameof(RecordStockInCommand.Quantity));

    [Test]
    public void ShouldNotHaveErrors_ForAValidStockIn()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
