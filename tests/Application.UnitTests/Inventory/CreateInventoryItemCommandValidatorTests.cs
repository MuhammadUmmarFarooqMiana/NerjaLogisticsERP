using NerjaLogisticsERP.Application.Inventory.Commands.CreateInventoryItem;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Inventory;

public class CreateInventoryItemCommandValidatorTests
{
    private readonly CreateInventoryItemCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenItemNameIsEmpty()
        => _validator.Validate(new CreateInventoryItemCommand { ItemName = "", ReorderLevel = 5 })
            .ShouldHaveErrorFor(nameof(CreateInventoryItemCommand.ItemName));

    [Test]
    public void ShouldHaveError_WhenItemNameExceedsMaxLength()
        => _validator.Validate(new CreateInventoryItemCommand { ItemName = new string('x', 151), ReorderLevel = 5 })
            .ShouldHaveErrorFor(nameof(CreateInventoryItemCommand.ItemName));

    [Test]
    public void ShouldHaveError_WhenReorderLevelIsNegative()
        => _validator.Validate(new CreateInventoryItemCommand { ItemName = "Brake Pads", ReorderLevel = -1 })
            .ShouldHaveErrorFor(nameof(CreateInventoryItemCommand.ReorderLevel));

    [Test]
    public void ShouldNotHaveErrors_ForAValidItem()
        => _validator.Validate(new CreateInventoryItemCommand { ItemName = "Brake Pads", ReorderLevel = 5 })
            .IsValid.ShouldBeTrue();
}
