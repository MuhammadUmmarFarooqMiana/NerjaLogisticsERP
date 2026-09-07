using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.VerifyMonthlySummary;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.MonthlySummaries;

public class GenerateMonthlySummaryCommandTests : TestBase
{
    private static readonly DateOnly FormulaEffectiveFrom = new(2026, 1, 1);
    private const int Year = 2026;
    private const int Month = 7;

    [Test]
    public async Task ShouldGenerateSummary_UsingFixedMonthlyFormula()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await MonthlySummaryTestHelpers.CreateOrderAsync(employeeId, new DateOnly(Year, Month, 5), 40, DailyOrderStatus.Approved);
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        var summaryId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });

        var summary = await TestApp.FindAsync<MonthlySummary>(summaryId);
        summary!.Status.ShouldBe(MonthlySummaryStatus.Draft);
        summary.TotalSalary.ShouldBe(2500m); // FixedMonthly ignores order count entirely
        summary.TotalCompletedOrders.ShouldBe(40);
    }

    [Test]
    public async Task ShouldSumOnlyApprovedOrders_ExcludingRejected()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await MonthlySummaryTestHelpers.CreateOrderAsync(employeeId, new DateOnly(Year, Month, 5), 10, DailyOrderStatus.Approved);
        await MonthlySummaryTestHelpers.CreateOrderAsync(employeeId, new DateOnly(Year, Month, 6), 15, DailyOrderStatus.Approved);
        await MonthlySummaryTestHelpers.CreateOrderAsync(employeeId, new DateOnly(Year, Month, 7), 100, DailyOrderStatus.Rejected);
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        var summaryId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });

        (await TestApp.FindAsync<MonthlySummary>(summaryId))!.TotalCompletedOrders.ShouldBe(25); // 10 + 15, not +100
    }

    // Proves real integration with SalaryCalculator (already unit-tested on its own), not just
    // that the handler can hand back a hardcoded FixedMonthly amount.
    [Test]
    public async Task ShouldApplyTieredPerOrderFormula_ViaTheRealSalaryCalculator()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddTieredPerOrderFormulaAsync(platformId, FormulaEffectiveFrom, ratePerOrder: 10m);
        await MonthlySummaryTestHelpers.CreateOrderAsync(employeeId, new DateOnly(Year, Month, 5), 40, DailyOrderStatus.Approved);
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        var summaryId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });

        (await TestApp.FindAsync<MonthlySummary>(summaryId))!.TotalSalary.ShouldBe(400m); // 40 orders * 10/order
    }

    [Test]
    public async Task ShouldPreferPlatformSpecificFormula_OverGenericDefault()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 3000m, FormulaEffectiveFrom);   // platform-specific
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(null, 1000m, FormulaEffectiveFrom);         // generic default
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        var summaryId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });

        (await TestApp.FindAsync<MonthlySummary>(summaryId))!.TotalSalary.ShouldBe(3000m);
    }

    [Test]
    public async Task ShouldFallBackToGenericFormula_WhenNoPlatformSpecificFormulaExists()
    {
        var (employeeId, _) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(null, 1000m, FormulaEffectiveFrom); // generic only
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        var summaryId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });

        (await TestApp.FindAsync<MonthlySummary>(summaryId))!.TotalSalary.ShouldBe(1000m);
    }

    [Test]
    public async Task ShouldThrowNotFound_WhenNoApplicableSalaryFormulaExists()
    {
        var (employeeId, _) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        // no formula at all — neither platform-specific nor generic
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month }));
    }

    [Test]
    public async Task ShouldThrowConflict_WhenClosedOrdersAreStillPendingApproval()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await MonthlySummaryTestHelpers.CreateOrderAsync(employeeId, new DateOnly(Year, Month, 10), 20, DailyOrderStatus.Closed); // never reviewed
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        await Should.ThrowAsync<ConflictException>(async () =>
            await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month }));
    }

    [Test]
    public async Task ShouldSumAdvancesAndFines_ForTheTargetMonthOnly()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await TestApp.AddAsync(Advance.Create(employeeId, 200m, new DateOnly(Year, Month, 3), null, Guid.NewGuid()));
        await TestApp.AddAsync(Fine.Create(employeeId, 50m, "Late.", new DateOnly(Year, Month, 4), Guid.NewGuid()));
        // a different month — must NOT be counted
        await TestApp.AddAsync(Advance.Create(employeeId, 9999m, new DateOnly(Year, Month - 1, 15), null, Guid.NewGuid()));
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        var summaryId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });

        var summary = await TestApp.FindAsync<MonthlySummary>(summaryId);
        summary!.TotalAdvances.ShouldBe(200m);
        summary.TotalFines.ShouldBe(50m);
        summary.NetSalaryPayable.ShouldBe(2500m - 200m - 50m);
    }

    [Test]
    public async Task ShouldReplaceExistingDraftSummary_WhenRegeneratedBeforeVerification()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await MonthlySummaryTestHelpers.CreateOrderAsync(employeeId, new DateOnly(Year, Month, 5), 10, DailyOrderStatus.Approved);
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);
        var firstId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });

        // more orders come in for the same month before anyone verifies the draft
        await MonthlySummaryTestHelpers.CreateOrderAsync(employeeId, new DateOnly(Year, Month, 20), 15, DailyOrderStatus.Approved);
        var secondId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });

        secondId.ShouldNotBe(firstId); // Remove+Add, not an in-place update — a genuinely new row
        (await TestApp.CountAsync<MonthlySummary>()).ShouldBe(1);
        (await TestApp.FindAsync<MonthlySummary>(secondId))!.TotalCompletedOrders.ShouldBe(25);
    }

    [Test]
    public async Task ShouldThrowConflict_WhenRegeneratingAnAlreadyVerifiedSummary()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);
        var summaryId = await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month });
        await TestApp.SendAsync(new VerifyMonthlySummaryCommand { Id = summaryId });

        await Should.ThrowAsync<ConflictException>(async () =>
            await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month }));
    }

    [Test]
    public async Task ShouldThrowNotFound_WhenEmployeeDoesNotExist()
    {
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = Guid.NewGuid(), Year = Year, Month = Month }));
    }

    [Test]
    public async Task ShouldSucceed_WhenActorIsAdministrator()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Administrator);

        await Should.NotThrowAsync(async () =>
            await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = Year, Month = Month }));
    }
}
