using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.VerifyMonthlySummary;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.MonthlySummaries;

public class VerifyMonthlySummaryCommandTests : TestBase
{
    private static readonly DateOnly FormulaEffectiveFrom = new(2026, 1, 1);

    private static async Task<Guid> CreateDraftSummaryAsync()
    {
        var (employeeId, platformId) = await MonthlySummaryTestHelpers.CreateRiderOnPlatformAsync();
        await MonthlySummaryTestHelpers.AddFixedMonthlyFormulaAsync(platformId, 2500m, FormulaEffectiveFrom);
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);
        return await TestApp.SendAsync(new GenerateMonthlySummaryCommand { EmployeeId = employeeId, Year = 2026, Month = 7 });
    }

    [Test]
    public async Task ShouldVerify_WhenDraft()
    {
        var summaryId = await CreateDraftSummaryAsync();

        await TestApp.SendAsync(new VerifyMonthlySummaryCommand { Id = summaryId });

        var summary = await TestApp.FindAsync<MonthlySummary>(summaryId);
        summary!.Status.ShouldBe(MonthlySummaryStatus.Verified);
        summary.VerifiedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task ShouldThrowNotFound_WhenSummaryDoesNotExist()
    {
        await MonthlySummaryTestHelpers.RunAsAsync(Roles.Accountant);

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync(new VerifyMonthlySummaryCommand { Id = Guid.NewGuid() }));
    }
}
