using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Application.Users.Commands.UpdateUserRoles;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Users;

public class UpdateUserRolesCommandValidatorTests
{
    private readonly UpdateUserRolesCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenUserIdIsEmpty()
        => _validator.Validate(new UpdateUserRolesCommand { UserId = Guid.Empty, Roles = ["Rider"] })
            .ShouldHaveErrorFor(nameof(UpdateUserRolesCommand.UserId));

    [Test]
    public void ShouldHaveError_WhenNoRolesAreProvided()
        => _validator.Validate(new UpdateUserRolesCommand { UserId = Guid.NewGuid(), Roles = [] })
            .ShouldHaveErrorFor(nameof(UpdateUserRolesCommand.Roles));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRequest()
        => _validator.Validate(new UpdateUserRolesCommand { UserId = Guid.NewGuid(), Roles = ["Rider"] })
            .IsValid.ShouldBeTrue();
}
