using NerjaLogisticsERP.Application.CompanyDocuments.Commands.UploadCompanyDocument;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.CompanyDocuments;

public class UploadCompanyDocumentCommandValidatorTests
{
    private readonly UploadCompanyDocumentCommandValidator _validator = new();

    private static UploadCompanyDocumentCommand Valid() => new()
    {
        Title = "Vehicle Insurance Policy",
        Category = CompanyDocumentCategory.Legal,
        Content = [1, 2, 3],
        FileName = "policy.pdf",
        ContentType = "application/pdf"
    };

    [Test]
    public void ShouldHaveError_WhenTitleIsEmpty()
        => _validator.Validate(Valid() with { Title = "" })
            .ShouldHaveErrorFor(nameof(UploadCompanyDocumentCommand.Title));

    [Test]
    public void ShouldHaveError_WhenTitleExceedsMaxLength()
        => _validator.Validate(Valid() with { Title = new string('x', 201) })
            .ShouldHaveErrorFor(nameof(UploadCompanyDocumentCommand.Title));

    [Test]
    public void ShouldHaveError_WhenCategoryIsNotAValidEnumValue()
        => _validator.Validate(Valid() with { Category = (CompanyDocumentCategory)999 })
            .ShouldHaveErrorFor(nameof(UploadCompanyDocumentCommand.Category));

    [Test]
    public void ShouldHaveError_WhenFileNameIsEmpty()
        => _validator.Validate(Valid() with { FileName = "" })
            .ShouldHaveErrorFor(nameof(UploadCompanyDocumentCommand.FileName));

    [Test]
    public void ShouldHaveError_WhenFileTypeIsNotAllowed()
        => _validator.Validate(Valid() with { FileName = "notes.txt", ContentType = "text/plain" })
            .IsValid.ShouldBeFalse();

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpload()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
