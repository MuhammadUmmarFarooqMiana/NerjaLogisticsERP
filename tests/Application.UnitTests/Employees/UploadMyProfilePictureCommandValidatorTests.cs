using NerjaLogisticsERP.Application.Employees.Commands.UploadMyProfilePicture;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Employees;

public class UploadMyProfilePictureCommandValidatorTests
{
    private readonly UploadMyProfilePictureCommandValidator _validator = new();

    private static UploadMyProfilePictureCommand Valid() => new()
    {
        Content = [1, 2, 3],
        FileName = "profile.jpg",
        ContentType = "image/jpeg"
    };

    [Test]
    public void ShouldFail_WhenFileNameIsEmpty()
        => _validator.Validate(Valid() with { FileName = "" })
            .IsValid.ShouldBeFalse();

    // Narrower than the general document-upload rule: only JPEG/PNG are accepted for a profile
    // picture, even though PDF/Word/Excel are all otherwise "allowed" file types in this app.
    [Test]
    public void ShouldFail_WhenContentTypeIsNotAPhoto()
        => _validator.Validate(Valid() with { FileName = "profile.pdf", ContentType = "application/pdf" })
            .IsValid.ShouldBeFalse();

    [TestCase("profile.jpg", "image/jpeg")]
    [TestCase("profile.png", "image/png")]
    public void ShouldSucceed_ForJpegOrPngPhotos(string fileName, string contentType)
        => _validator.Validate(Valid() with { FileName = fileName, ContentType = contentType })
            .IsValid.ShouldBeTrue();
}
