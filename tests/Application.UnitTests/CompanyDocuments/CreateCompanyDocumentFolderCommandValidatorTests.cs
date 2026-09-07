using NerjaLogisticsERP.Application.CompanyDocuments.Commands.CreateCompanyDocumentFolder;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.CompanyDocuments;

public class CreateCompanyDocumentFolderCommandValidatorTests
{
    private readonly CreateCompanyDocumentFolderCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenNameIsEmpty()
        => _validator.Validate(new CreateCompanyDocumentFolderCommand { Name = "" })
            .ShouldHaveErrorFor(nameof(CreateCompanyDocumentFolderCommand.Name));

    [Test]
    public void ShouldHaveError_WhenNameExceedsMaxLength()
        => _validator.Validate(new CreateCompanyDocumentFolderCommand { Name = new string('x', 151) })
            .ShouldHaveErrorFor(nameof(CreateCompanyDocumentFolderCommand.Name));

    [Test]
    public void ShouldNotHaveErrors_ForAValidFolder()
        => _validator.Validate(new CreateCompanyDocumentFolderCommand { Name = "Contracts 2026" })
            .IsValid.ShouldBeTrue();
}
