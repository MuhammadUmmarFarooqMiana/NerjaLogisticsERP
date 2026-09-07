using NerjaLogisticsERP.Application.Platforms.Commands.UpdatePlatform;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Platforms;

public class UpdatePlatformCommandValidatorTests
{
    private readonly UpdatePlatformCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(new UpdatePlatformCommand { Id = Guid.Empty, Name = "Hunger" })
            .ShouldHaveErrorFor(nameof(UpdatePlatformCommand.Id));

    [Test]
    public void ShouldHaveError_WhenNameIsEmpty()
        => _validator.Validate(new UpdatePlatformCommand { Id = Guid.NewGuid(), Name = "" })
            .ShouldHaveErrorFor(nameof(UpdatePlatformCommand.Name));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(new UpdatePlatformCommand { Id = Guid.NewGuid(), Name = "Hunger" })
            .IsValid.ShouldBeTrue();
}
