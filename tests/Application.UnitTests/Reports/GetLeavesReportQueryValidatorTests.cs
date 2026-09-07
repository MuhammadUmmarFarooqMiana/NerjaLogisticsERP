using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Application.Reports.Leaves.Queries.GetLeavesReport;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Reports;

public class GetLeavesReportQueryValidatorTests
{
    private readonly GetLeavesReportQueryValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenCustomPeriodIsMissingStartDate()
        => _validator.Validate(new GetLeavesReportQuery { PeriodType = ReportPeriodType.Custom, EndDate = new DateOnly(2026, 7, 31) })
            .ShouldHaveErrorFor(nameof(GetLeavesReportQuery.StartDate));

    [Test]
    public void ShouldHaveError_WhenCustomPeriodIsMissingEndDate()
        => _validator.Validate(new GetLeavesReportQuery { PeriodType = ReportPeriodType.Custom, StartDate = new DateOnly(2026, 7, 1) })
            .ShouldHaveErrorFor(nameof(GetLeavesReportQuery.EndDate));

    [TestCase(0)]
    [TestCase(13)]
    public void ShouldHaveError_WhenMonthIsOutOfRange(int month)
        => _validator.Validate(new GetLeavesReportQuery { PeriodType = ReportPeriodType.Monthly, Month = month })
            .ShouldHaveErrorFor(nameof(GetLeavesReportQuery.Month));

    [Test]
    public void ShouldNotHaveErrors_ForAMonthlyPeriodWithNoExplicitDates()
        => _validator.Validate(new GetLeavesReportQuery { PeriodType = ReportPeriodType.Monthly })
            .IsValid.ShouldBeTrue();

    [Test]
    public void ShouldNotHaveErrors_ForACustomPeriodWithBothDates()
        => _validator.Validate(new GetLeavesReportQuery
            {
                PeriodType = ReportPeriodType.Custom,
                StartDate = new DateOnly(2026, 7, 1),
                EndDate = new DateOnly(2026, 7, 31)
            })
            .IsValid.ShouldBeTrue();
}
