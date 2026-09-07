using NerjaLogisticsERP.Application.Mechanics.Commands.CreateMechanic;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Mechanics;

public class CreateMechanicCommandValidatorTests
{
    private readonly CreateMechanicCommandValidator _validator = new();

    private static CreateMechanicCommand Valid() => new() { Name = "Ali's Garage" };

    [Test]
    public void ShouldHaveError_WhenNameIsEmpty()
        => _validator.Validate(Valid() with { Name = "" })
            .ShouldHaveErrorFor(nameof(CreateMechanicCommand.Name));

    [Test]
    public void ShouldHaveError_WhenEmailIsProvidedButInvalid()
        => _validator.Validate(Valid() with { Email = "not-an-email" })
            .ShouldHaveErrorFor(nameof(CreateMechanicCommand.Email));

    [Test]
    public void ShouldNotHaveError_WhenEmailIsBlank()
        => _validator.Validate(Valid() with { Email = null })
            .ShouldNotHaveErrorFor(nameof(CreateMechanicCommand.Email));

    [Test]
    public void ShouldNotHaveErrors_ForAValidMechanic()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
