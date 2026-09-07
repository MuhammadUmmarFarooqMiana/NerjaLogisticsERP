using NerjaLogisticsERP.Application.Suppliers.Commands.UpdateSupplier;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Suppliers;

public class UpdateSupplierCommandValidatorTests
{
    private readonly UpdateSupplierCommandValidator _validator = new();

    private static UpdateSupplierCommand Valid() => new() { Id = Guid.NewGuid(), Name = "Acme Parts Co." };

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(Valid() with { Id = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateSupplierCommand.Id));

    [Test]
    public void ShouldHaveError_WhenNameIsEmpty()
        => _validator.Validate(Valid() with { Name = "" })
            .ShouldHaveErrorFor(nameof(UpdateSupplierCommand.Name));

    [Test]
    public void ShouldHaveError_WhenEmailIsProvidedButInvalid()
        => _validator.Validate(Valid() with { Email = "not-an-email" })
            .ShouldHaveErrorFor(nameof(UpdateSupplierCommand.Email));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
