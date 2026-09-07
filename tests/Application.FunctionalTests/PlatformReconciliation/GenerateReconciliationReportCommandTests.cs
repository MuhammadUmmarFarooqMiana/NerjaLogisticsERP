using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.PlatformReconciliation.Commands.GenerateReconciliationReport;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.FunctionalTests.PlatformReconciliation;

public class GenerateReconciliationReportCommandTests : TestBase
{
    private const int Year = 2026;
    private const int Month = 7;

    private static PlatformRiderRow Row(
        string riderId, int completedOrders,
        decimal stackingDeduction = 0m, decimal declinedPenaltiesDayLogic = 0m, decimal latePenalty = 0m,
        decimal noShowPenalty = 0m, decimal noShowPenaltySpecialCities = 0m,
        decimal dailyAcceptanceRatePenalty = 0m, decimal missedDaysPenalty = 0m)
        => new()
        {
            RiderId = riderId,
            CompletedOrders = completedOrders,
            StackingDeduction = stackingDeduction,
            DeclinedPenaltiesDayLogic = declinedPenaltiesDayLogic,
            LatePenalty = latePenalty,
            NoShowPenalty = noShowPenalty,
            NoShowPenaltySpecialCities = noShowPenaltySpecialCities,
            DailyAcceptanceRatePenalty = dailyAcceptanceRatePenalty,
            MissedDaysPenalty = missedDaysPenalty
        };

    [Test]
    public async Task ShouldMatchRider_ByPlatformIdNumber_AndComputeAdjustedNetPayable()
    {
        var platformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        await PlatformReconciliationTestHelpers.CreateEmployeeWithSummaryAsync(
            platformId, "RIDER001", Year, Month, totalCompletedOrders: 100, totalSalary: 2500m);
        // penalties are reported already-negative by the platform, matching real payout sheets
        TestApp.SetReconciliationRows([Row("RIDER001", completedOrders: 100, stackingDeduction: -50m)]);
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);

        var report = await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = platformId, Year = Year, Month = Month });

        report.MatchedRows.Count.ShouldBe(1);
        var row = report.MatchedRows[0];
        row.OrdersDifference.ShouldBe(0);
        row.TotalPenalties.ShouldBe(-50m);
        row.OriginalNetPayable.ShouldBe(2500m);
        row.AdjustedNetPayable.ShouldBe(2450m); // 2500 + (-50)
        report.UnmatchedPlatformRows.ShouldBeEmpty();
        report.MissingFromSheetRows.ShouldBeEmpty();
    }

    [Test]
    public async Task ShouldMatch_CaseInsensitively_OnPlatformIdNumber()
    {
        var platformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        await PlatformReconciliationTestHelpers.CreateEmployeeWithSummaryAsync(
            platformId, "rider001", Year, Month, totalCompletedOrders: 10, totalSalary: 100m);
        TestApp.SetReconciliationRows([Row("RIDER001", completedOrders: 10)]); // different case
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);

        var report = await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = platformId, Year = Year, Month = Month });

        report.MatchedRows.Count.ShouldBe(1);
    }

    [Test]
    public async Task ShouldFlagOrdersDifference_AsPlatformMinusNerja()
    {
        var platformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        await PlatformReconciliationTestHelpers.CreateEmployeeWithSummaryAsync(
            platformId, "RIDER001", Year, Month, totalCompletedOrders: 100, totalSalary: 2500m);
        TestApp.SetReconciliationRows([Row("RIDER001", completedOrders: 110)]);
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);

        var report = await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = platformId, Year = Year, Month = Month });

        report.MatchedRows[0].OrdersDifference.ShouldBe(10); // 110 - 100
        report.TotalOrdersMismatchCount.ShouldBe(1);
    }

    [Test]
    public async Task ShouldListUnmatchedPlatformRow_WhenRiderIdMatchesNoEmployee()
    {
        var platformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        TestApp.SetReconciliationRows([Row("GHOST999", completedOrders: 50, latePenalty: -20m)]);
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);

        var report = await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = platformId, Year = Year, Month = Month });

        report.MatchedRows.ShouldBeEmpty();
        report.UnmatchedPlatformRows.Count.ShouldBe(1);
        report.UnmatchedPlatformRows[0].PlatformIdNumber.ShouldBe("GHOST999");
        report.UnmatchedPlatformRows[0].TotalPenalties.ShouldBe(-20m);
    }

    [Test]
    public async Task ShouldListMissingFromSheetRow_WhenEmployeeHasSummaryButSheetHasNoMatchingRow()
    {
        var platformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        await PlatformReconciliationTestHelpers.CreateEmployeeWithSummaryAsync(
            platformId, "RIDER002", Year, Month, totalCompletedOrders: 80, totalSalary: 2000m);
        TestApp.SetReconciliationRows([]); // sheet says nothing about this rider at all
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);

        var report = await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = platformId, Year = Year, Month = Month });

        report.MatchedRows.ShouldBeEmpty();
        report.MissingFromSheetRows.Count.ShouldBe(1);
        report.MissingFromSheetRows[0].NerjaCompletedOrders.ShouldBe(80);
        report.MissingFromSheetRows[0].NetPayable.ShouldBe(2000m);
    }

    // A distinct code path from the test above: PlatformIdNumber is null (not just
    // sheet-absent), which the handler's IsNullOrWhiteSpace branch has to catch too.
    [Test]
    public async Task ShouldListMissingFromSheetRow_WhenEmployeeHasNoPlatformIdNumberAtAll()
    {
        var platformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        await PlatformReconciliationTestHelpers.CreateEmployeeWithSummaryAsync(
            platformId, platformIdNumber: null, Year, Month, totalCompletedOrders: 30, totalSalary: 900m);
        TestApp.SetReconciliationRows([]);
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);

        var report = await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = platformId, Year = Year, Month = Month });

        report.MissingFromSheetRows.Count.ShouldBe(1);
        report.MissingFromSheetRows[0].PlatformIdNumber.ShouldBeNull();
    }

    [Test]
    public async Task ShouldExcludeEmployeesOnADifferentPlatform_Entirely()
    {
        var targetPlatformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        var otherPlatformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        await PlatformReconciliationTestHelpers.CreateEmployeeWithSummaryAsync(
            otherPlatformId, "OTHERPLATFORMRIDER", Year, Month, totalCompletedOrders: 999, totalSalary: 9999m);
        TestApp.SetReconciliationRows([]);
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);

        var report = await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = targetPlatformId, Year = Year, Month = Month });

        report.MatchedRows.ShouldBeEmpty();
        report.MissingFromSheetRows.ShouldBeEmpty(); // the other platform's employee must not leak in here
    }

    [Test]
    public async Task ShouldSumTotalsAcrossAllMatchedRows()
    {
        var platformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        await PlatformReconciliationTestHelpers.CreateEmployeeWithSummaryAsync(platformId, "RIDER001", Year, Month, 100, 2500m);
        await PlatformReconciliationTestHelpers.CreateEmployeeWithSummaryAsync(platformId, "RIDER002", Year, Month, 50, 1200m);
        TestApp.SetReconciliationRows([
            Row("RIDER001", completedOrders: 100, stackingDeduction: -50m),
            Row("RIDER002", completedOrders: 60, latePenalty: -30m) // also a mismatch: 60 vs 50
        ]);
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);

        var report = await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = platformId, Year = Year, Month = Month });

        report.TotalMatched.ShouldBe(2);
        report.TotalOrdersMismatchCount.ShouldBe(1); // only RIDER002 disagrees
        report.TotalPenalties.ShouldBe(-80m);        // -50 + -30
        report.TotalOriginalNetPayable.ShouldBe(3700m);  // 2500 + 1200
        report.TotalAdjustedNetPayable.ShouldBe(3620m);  // 3700 + (-80)
    }

    [Test]
    public async Task ShouldThrowNotFound_WhenPlatformDoesNotExist()
    {
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Accountant);
        TestApp.SetReconciliationRows([]);

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = Guid.NewGuid(), Year = Year, Month = Month }));
    }

    [Test]
    public async Task ShouldSucceed_WhenActorIsAdministrator()
    {
        var platformId = await PlatformReconciliationTestHelpers.CreatePlatformAsync();
        TestApp.SetReconciliationRows([]);
        await PlatformReconciliationTestHelpers.RunAsAsync(Roles.Administrator);

        await Should.NotThrowAsync(async () =>
            await TestApp.SendAsync(new GenerateReconciliationReportCommand { Content = [1], FileName = "test.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PlatformId = platformId, Year = Year, Month = Month }));
    }
}
