using NerjaLogisticsERP.Application.Mechanics.Commands.UpdateMechanic;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Mechanics;

public class UpdateMechanicCommandValidatorTests
{
    private readonly UpdateMechanicCommandValidator _validator = new();

    private static UpdateMechanicCommand Valid() => new() { Id = Guid.NewGuid(), Name = "Ali's Garage" };

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(Valid() with { Id = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateMechanicCommand.Id));

    [Test]
    public void ShouldHaveError_WhenNameIsEmpty()
        => _validator.Validate(Valid() with { Name = "" })
            .ShouldHaveErrorFor(nameof(UpdateMechanicCommand.Name));

    [Test]
    public void ShouldHaveError_WhenEmailIsProvidedButInvalid()
        => _validator.Validate(Valid() with { Email = "not-an-email" })
            .ShouldHaveErrorFor(nameof(UpdateMechanicCommand.Email));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
