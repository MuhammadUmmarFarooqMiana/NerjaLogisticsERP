using NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.ExportReconciliationReport;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.PlatformReconciliation;

public class ExportReconciliationReportCommandValidatorTests
{
    private readonly ExportReconciliationReportCommandValidator _validator = new();

    private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private static ExportReconciliationReportCommand Valid() => new()
    {
        Content = [1, 2, 3],
        FileName = "payout.xlsx",
        ContentType = XlsxContentType,
        PlatformId = Guid.NewGuid(),
        Year = 2026,
        Month = 7
    };

    [Test]
    public void ShouldHaveError_WhenFileNameIsEmpty()
        => _validator.Validate(Valid() with { FileName = "" })
            .ShouldHaveErrorFor(nameof(ExportReconciliationReportCommand.FileName));

    // The rule is really "any of the app's generally-allowed file types" (PDF/Word/Excel/images
    // — see AllowedFileTypes), not specifically Excel, despite what the error message claims. A
    // .pdf would actually pass this rule; a genuinely unlisted type like .txt is what fails it.
    [Test]
    public void ShouldHaveError_WhenFileTypeIsNotInTheAllowedList()
        => _validator.Validate(Valid() with { FileName = "payout.txt", ContentType = "text/plain" })
            .IsValid.ShouldBeFalse();

    [Test]
    public void ShouldHaveError_WhenPlatformIdIsEmpty()
        => _validator.Validate(Valid() with { PlatformId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(ExportReconciliationReportCommand.PlatformId));

    [TestCase(0)]
    [TestCase(13)]
    public void ShouldHaveError_WhenMonthIsOutOfRange(int month)
        => _validator.Validate(Valid() with { Month = month })
            .ShouldHaveErrorFor(nameof(ExportReconciliationReportCommand.Month));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRequest()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
