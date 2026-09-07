using NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Employees;

public class SubmitProfileForReviewCommandValidatorTests
{
    private readonly SubmitProfileForReviewCommandValidator _validator = new();

    private static SubmitProfileForReviewCommand Valid() => new()
    {
        UserId = Guid.NewGuid(),
        IqamaNumber = "1234567890",
        PlatformIdNumber = "RIDER001"
    };

    [Test]
    public void ShouldHaveError_WhenUserIdIsEmpty()
        => _validator.Validate(Valid() with { UserId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(SubmitProfileForReviewCommand.UserId));

    [Test]
    public void ShouldHaveError_WhenIqamaNumberIsEmpty()
        => _validator.Validate(Valid() with { IqamaNumber = "" })
            .ShouldHaveErrorFor(nameof(SubmitProfileForReviewCommand.IqamaNumber));

    [Test]
    public void ShouldHaveError_WhenIqamaNumberExceedsMaxLength()
        => _validator.Validate(Valid() with { IqamaNumber = new string('1', 31) })
            .ShouldHaveErrorFor(nameof(SubmitProfileForReviewCommand.IqamaNumber));

    [Test]
    public void ShouldHaveError_WhenPlatformIdNumberIsEmpty()
        => _validator.Validate(Valid() with { PlatformIdNumber = "" })
            .ShouldHaveErrorFor(nameof(SubmitProfileForReviewCommand.PlatformIdNumber));

    [Test]
    public void ShouldNotHaveErrors_ForAValidSubmission()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
