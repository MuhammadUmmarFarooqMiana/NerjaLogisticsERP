using NerjaLogisticsERP.Application.Suppliers.Commands.CreateSupplier;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Suppliers;

public class CreateSupplierCommandValidatorTests
{
    private readonly CreateSupplierCommandValidator _validator = new();

    private static CreateSupplierCommand Valid() => new() { Name = "Acme Parts Co." };

    [Test]
    public void ShouldHaveError_WhenNameIsEmpty()
        => _validator.Validate(Valid() with { Name = "" })
            .ShouldHaveErrorFor(nameof(CreateSupplierCommand.Name));

    [Test]
    public void ShouldHaveError_WhenNameExceedsMaxLength()
        => _validator.Validate(Valid() with { Name = new string('x', 251) })
            .ShouldHaveErrorFor(nameof(CreateSupplierCommand.Name));

    [Test]
    public void ShouldHaveError_WhenEmailIsProvidedButInvalid()
        => _validator.Validate(Valid() with { Email = "not-an-email" })
            .ShouldHaveErrorFor(nameof(CreateSupplierCommand.Email));

    [Test]
    public void ShouldNotHaveError_WhenEmailIsBlank()
        => _validator.Validate(Valid() with { Email = null })
            .ShouldNotHaveErrorFor(nameof(CreateSupplierCommand.Email));

    [Test]
    public void ShouldNotHaveErrors_ForAValidSupplier()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
