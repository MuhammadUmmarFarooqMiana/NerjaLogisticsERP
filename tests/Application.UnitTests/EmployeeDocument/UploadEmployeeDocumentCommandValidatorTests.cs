using NerjaLogisticsERP.Application.EmployeeDocument.Commands.UploadEmployeeDocument;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.EmployeeDocument;

public class UploadEmployeeDocumentCommandValidatorTests
{
    private readonly UploadEmployeeDocumentCommandValidator _validator = new();

    private static UploadEmployeeDocumentCommand Valid() => new()
    {
        Content = [1, 2, 3],
        FileName = "iqama.jpg",
        ContentType = "image/jpeg"
    };

    [Test]
    public void ShouldFail_WhenFileNameIsEmpty()
        => _validator.Validate(Valid() with { FileName = "" })
            .IsValid.ShouldBeFalse();

    [Test]
    public void ShouldFail_WhenFileTypeIsNotAllowed()
        => _validator.Validate(Valid() with { FileName = "notes.txt", ContentType = "text/plain" })
            .IsValid.ShouldBeFalse();

    [Test]
    public void ShouldSucceed_ForAnAllowedFileType()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
