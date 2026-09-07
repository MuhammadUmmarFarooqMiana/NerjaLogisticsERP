using NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.MonthlySummaries;

public class GenerateMonthlySummaryCommandValidatorTests
{
    private readonly GenerateMonthlySummaryCommandValidator _validator = new();

    private static GenerateMonthlySummaryCommand Valid() => new() { EmployeeId = Guid.NewGuid(), Year = 2026, Month = 7 };

    [Test]
    public void ShouldHaveError_WhenEmployeeIdIsEmpty()
        => _validator.Validate(Valid() with { EmployeeId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(GenerateMonthlySummaryCommand.EmployeeId));

    [TestCase(0)]
    [TestCase(13)]
    public void ShouldHaveError_WhenMonthIsOutOfRange(int month)
        => _validator.Validate(Valid() with { Month = month })
            .ShouldHaveErrorFor(nameof(GenerateMonthlySummaryCommand.Month));

    [Test]
    public void ShouldHaveError_WhenYearIsNotAfter2020()
        => _validator.Validate(Valid() with { Year = 2020 })
            .ShouldHaveErrorFor(nameof(GenerateMonthlySummaryCommand.Year));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRequest()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
