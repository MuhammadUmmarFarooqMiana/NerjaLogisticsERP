using NerjaLogisticsERP.Application.Platforms.Commands.CreatePlatform;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Platforms;

public class CreatePlatformCommandValidatorTests
{
    private readonly CreatePlatformCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenNameIsEmpty()
        => _validator.Validate(new CreatePlatformCommand { Name = "" })
            .ShouldHaveErrorFor(nameof(CreatePlatformCommand.Name));

    [Test]
    public void ShouldHaveError_WhenNameExceedsMaxLength()
        => _validator.Validate(new CreatePlatformCommand { Name = new string('x', 51) })
            .ShouldHaveErrorFor(nameof(CreatePlatformCommand.Name));

    [Test]
    public void ShouldNotHaveErrors_ForAValidPlatform()
        => _validator.Validate(new CreatePlatformCommand { Name = "Hunger" })
            .IsValid.ShouldBeTrue();
}
